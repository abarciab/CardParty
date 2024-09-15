using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CombatSlot : MonoBehaviour
{
    [HideInInspector] public CreatureObject Creature { get; private set; }

    public bool IsBlockSlot = false;
    [HideInInspector] public List<AttackArrow> Arrows = new List<AttackArrow>();
    [SerializeField] private GameObject _model;
    [SerializeField] private bool _debug;

    [SerializeField, ConditionalField(nameof(_debug)), ReadOnly] private string _creatureName;

    [HideInInspector] public EnemyObject EnemyObject => IsBlockSlot ? _enemyObject : null;
    [HideInInspector] public bool IsLocked => IncomingAttacks.Count > 0;
    [HideInInspector] public List<AttackArrow> IncomingAttacks;

    private TabletopController _controller;
    private EnemyObject _enemyObject;
    private bool _inCooldown;

    public void Reset()
    {
        if (Creature) Creature.CombatSlot = null;
        SetCreature(null);
        IncomingAttacks.Clear();
        _creatureName = "null";
    }

    public void Initialize(GameObject creaturePrefab, TabletopController controller)
    {
        var creatureObject = Instantiate(creaturePrefab, transform);
        Creature = creatureObject.GetComponent<CreatureObject>();
        Creature.Initialize(controller);
        Creature.CombatSlot = this;
        Initialize(controller);
    }

    public void Initialize(TabletopController controller, EnemyObject _enemy)
    {
        _enemyObject = _enemy;
        IsBlockSlot = true;
        if (IsBlockSlot) gameObject.name = "Block Slot";
        Initialize(controller);
    }

    public void Initialize(TabletopController controller)
    {
        _controller = controller;
        Arrows = new List<AttackArrow>();
    }

    public void SetCreature(CreatureObject newCreature) {
        if (_inCooldown || newCreature == Creature) return;
        
        var oldCreature = Creature;
        Creature = newCreature;

        if (newCreature) {
            CombatSlot previousSlot = newCreature.CombatSlot;
            previousSlot?.SetCreature(oldCreature);

            newCreature.CombatSlot = this;
            newCreature.transform.SetParent(transform);
            newCreature.transform.localPosition = Vector3.zero;
        }

        foreach (var a in Arrows) a.UpdateVisuals();
        _creatureName = Creature == null ? "null" : Creature.GetName();

        /*_inCooldown = true;
        await Task.Delay((int)(moveCooldown * 1000));
        _inCooldown = false;*/
    }

    public void MoveCreature() {
        if (!Creature) return;
        SetCreature(Creature);
    }
    public void HideVisuals() => _model.SetActive(false);
}