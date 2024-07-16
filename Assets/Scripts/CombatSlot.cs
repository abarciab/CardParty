using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSlot : MonoBehaviour
{
    public Creature Creature = null;

    public bool IsBlockSlot = false;
    public AttackArrow AttackArrow;

    public void SetCreature(Creature creature, CombatSlot targetSlot = null) {
        if (!targetSlot) targetSlot = this;

        if (targetSlot.Creature) {
            creature.CombatSlot.Creature = targetSlot.Creature;
            targetSlot.Creature.CombatSlot = creature.CombatSlot;
            targetSlot.Creature.CombatSlot.Creature = null;
            targetSlot.Creature.transform.SetParent(targetSlot.Creature.CombatSlot.transform);
            targetSlot.Creature.transform.localPosition = Vector3.zero;
        }

        targetSlot.Creature = creature;
        creature.CombatSlot = this;
        creature.transform.SetParent(transform);
        creature.transform.localPosition = Vector3.zero;

        if (IsBlockSlot) {
            CardGameManager.i.UpdateAttackArrow(targetSlot);
        }
    }

    public void MoveCreature() {
        if (!Creature) return;
        SetCreature(Creature, targetSlot: CardGameManager.i.GetRandomAdventurerSlot(empty: true));
    }
}