using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletopController : MonoBehaviour
{
    private List<Adventurer> _adventurers = new List<Adventurer>();
    private List<Enemy> _enemies = new List<Enemy>();
    private List<CombatSlot> _adventurerCombatSlots = new List<CombatSlot>();
    private List<CombatSlot> _enemyCombatSlots = new List<CombatSlot>();
    private List<CombatSlot> _blockCombatSlots = new List<CombatSlot>();
}
