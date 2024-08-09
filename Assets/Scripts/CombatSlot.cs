using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSlot : MonoBehaviour
{
    [HideInInspector] public CreatureObject Creature { get; private set; }

    public bool IsBlockSlot = false;
    public AttackArrow AttackArrow;
    [SerializeField, ReadOnly] TabletopController _controller;
    [SerializeField] private GameObject _model;

    private EnemyObject _enemyObject;
    public EnemyObject EnemyObject => IsBlockSlot ? _enemyObject : null;

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
    }

    public void SetCreature(CreatureObject creature) {

        CombatSlot oldSlot = creature.CombatSlot;
        oldSlot.Creature = null;
        if (Creature) {
            oldSlot.SetCreature(Creature);
        }

        Creature = creature;
        creature.CombatSlot = this;

        creature.transform.SetParent(transform);
        creature.transform.localPosition = Vector3.zero;

        if (IsBlockSlot) _controller.UpdateAttackArrows(this);
        if (oldSlot.IsBlockSlot) _controller.UpdateAttackArrows(oldSlot);
    }

    public void MoveCreature() {
        if (!Creature) return;
        SetCreature(Creature);
    }
    public void HideVisuals() => _model.SetActive(false);
}