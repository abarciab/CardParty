using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSlot : MonoBehaviour
{
    [HideInInspector] public Creature Creature { get; private set; }

    public bool IsBlockSlot = false;
    public AttackArrow AttackArrow;
    TabletopController _controller;

    public void InitializeWithCreature(GameObject creaturePrefab, TabletopController controller)
    {
        _controller = controller;
        var creatureObject = Instantiate(creaturePrefab, transform);
        Creature = creatureObject.GetComponent<Creature>();
        Creature.Initialize(controller);
        Creature.CombatSlot = this;
    }

    public void SetCreature(Creature creature, CombatSlot targetSlot = null) {
        if (!targetSlot) targetSlot = this;

        CombatSlot oldSlot = creature.CombatSlot;
        oldSlot.Creature = null;
        if (targetSlot.Creature) {
            SetCreature(targetSlot.Creature, oldSlot);
        }

        targetSlot.Creature = creature;
        creature.CombatSlot = targetSlot;

        creature.transform.SetParent(targetSlot.transform);
        creature.transform.localPosition = Vector3.zero;
        
        if (targetSlot.IsBlockSlot) CardGameManager.i.UpdateAttackArrow(targetSlot);
        if (oldSlot.IsBlockSlot) CardGameManager.i.UpdateAttackArrow(oldSlot);
    }

    public void MoveCreature() {
        if (!Creature) return;
        SetCreature(Creature, targetSlot: _controller.GetRandomAdventurerSlot(empty: true));
    }
}