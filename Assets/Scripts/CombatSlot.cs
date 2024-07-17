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
        print(CardGameManager.i.GetRandomAdventurerSlot(empty: true));
        SetCreature(Creature, targetSlot: CardGameManager.i.GetRandomAdventurerSlot(empty: true));
    }
}