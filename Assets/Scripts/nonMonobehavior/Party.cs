using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Party
{
    public List<AdventurerData> Adventurers = new List<AdventurerData>();

    private Dictionary<AdventurerData, List<Equipment>> _equipmentDict = new Dictionary<AdventurerData, List<Equipment>>();

    public AdventurerData GetOwner(Equipment equipment)
    {
        foreach (var info in _equipmentDict) {
            if (info.Value.Contains(equipment)) return info.Key;
        }
        return null;
    }

    public AdventurerData GetOwner(CardData card)
    {
        foreach (var adventurer in Adventurers) {
            if (adventurer.Cards.Contains(card)) return adventurer;
        }
        return null;
    }

    public void SetParty(List<AdventurerData> adventurers)
    {
        Adventurers = adventurers;
    }
}
