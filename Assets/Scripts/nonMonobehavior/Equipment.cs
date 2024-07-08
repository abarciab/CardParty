using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Equipment")]
public class Equipment : ScriptableObject
{
    public string Name;
    [TextArea(3, 10)] public string Description;
    public EquipmentSlot Slot;
    public int Cost;
    [SerializeField, DisplayInspector] private List<CardData> _cards = new List<CardData>();

    public override string ToString()
    {
        return Name;
    }

    /*

    OnTurnStart(){
        if (!HasStartTurnBehavior) return;

        if (TURNSTARTBEHAV == HEALALLDUDES) HEALALLDUDES()
    }

    HEALALLDUDES() => GMan.healalldudes(amount)

    */
}
