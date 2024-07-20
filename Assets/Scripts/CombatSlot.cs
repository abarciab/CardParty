using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSlot : MonoBehaviour
{
    [HideInInspector] public Creature Creature { get; private set; }

    public bool IsBlockSlot = false;
    public AttackArrow AttackArrow;
    TabletopController _controller;
    [SerializeField] private GameObject _model;

    public void InitializeWithCreature(GameObject creaturePrefab, TabletopController controller)
    {
        _controller = controller;
        var creatureObject = Instantiate(creaturePrefab, transform);
        Creature = creatureObject.GetComponent<Creature>();
        Creature.Initialize(controller);
        Creature.CombatSlot = this;
    }

    public void SetCreature(Creature creature) {

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

    public void HideVisuals() => _model.SetActive(false);
}