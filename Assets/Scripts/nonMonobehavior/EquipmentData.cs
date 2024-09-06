using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public static EquipmentData GetEquipment(int difficulty = -1) {
        if (difficulty == -1) difficulty = Constants.MAX_DIFFICULTY;

        EquipmentRarity rarity = EquipmentRarity.Common;
        float r = UnityEngine.Random.Range(0, 1);
        if (difficulty < 5) {
            if (r < 0.3) rarity = EquipmentRarity.Uncommon;
            else rarity = EquipmentRarity.Common;
        } else if (difficulty <= 9) {
            if (r < 0.1) rarity = EquipmentRarity.Rare;
            else if (r < 0.4) rarity = EquipmentRarity.Uncommon;
            else rarity = EquipmentRarity.Common;
        } else if (difficulty <= 12) {
            if (r < 0.2) rarity = EquipmentRarity.Rare;
            else if (r < 0.5) rarity = EquipmentRarity.Uncommon;
            else rarity = EquipmentRarity.Common;
        }

        foreach(EquipmentData equipment in Resources.LoadAll<EquipmentData>( "Equipment/").ToList().Shuffle()) {
            if (equipment.Rarity == rarity) return equipment;
        }

        return null;
    }

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