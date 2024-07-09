using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Equipment")]
public class Equipment : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Description;
    public string WornByString = "Worn By";
    public EquipmentSlot Slot;
    public int Cost;
    [DisplayInspector] public List<CardData> Cards = new List<CardData>();

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object other)
    {
        var otherEquipment = other as Equipment;
        if (otherEquipment == null) return false;
        return string.Equals(ToString(), other.ToString());
    }

    /*

    OnTurnStart(){
        if (!HasStartTurnBehavior) return;

        if (TURNSTARTBEHAV == HEALALLDUDES) HEALALLDUDES()
    }

    HEALALLDUDES() => GMan.healalldudes(amount)

    */
}
