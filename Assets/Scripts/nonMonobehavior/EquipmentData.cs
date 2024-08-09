using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentRarity {Common, Uncommon, Rare}

[CreateAssetMenu(fileName ="EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Description;
    public string WornByString = "Worn By";
    public EquipmentSlot Slot;
    public int Cost;
    public EquipmentRarity Rarity;
    [DisplayInspector] public List<CardData> Cards = new List<CardData>();

    public AdventurerData Owner => PlayerInfo.Party.GetOwner(this);

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object other)
    {
        var otherEquipment = other as EquipmentData;
        if (otherEquipment == null) return false;
        return string.Equals(ToString(), other.ToString());
    }
}