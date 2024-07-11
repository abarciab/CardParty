using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Party
{
    public List<AdventurerData> Adventurers = new List<AdventurerData>();

    private Dictionary<AdventurerData, List<Equipment>> _equipmentDict = new Dictionary<AdventurerData, List<Equipment>>();
    private const int _ornamentIndex = 0;
    private const int _armorIndex = 1;
    private const int _mainIndex = 2;
    private const int _innateCardTotal = 6;

    public List<CardData> GetDeck()
    {
        var deck = new List<CardData>();

        foreach (var entry in _equipmentDict) {
            var adventurer = entry.Key;
            var equipmentList = entry.Value; 
            
            deck.AddRange(adventurer.GetInnateCards(_innateCardTotal));
            foreach (var equipment in equipmentList) {
                if (equipment != null) deck.AddRange(equipment.Cards);
            }
        }

        return deck;
    }

    public List<Equipment> GetAllEquippedItems()
    {
        var list = new List<Equipment>();
        foreach (var equipmentList in _equipmentDict.Values) {
            foreach (var e in equipmentList) if (e != null) list.Add(e); 
        }
        return list;
    }

    public Equipment GetCurrentEquipment(AdventurerData adventurer, EquipmentSlot slot)
    {
        int index = SlotToIndex(slot);
        return _equipmentDict[adventurer][index];
    }

    public AdventurerData GetOwner(Equipment equipment)
    {
        foreach (var info in _equipmentDict) {
            if (info.Value.Contains(equipment)) return info.Key;
        }
        return null;
    }

    public AdventurerData GetOwner(CardData card)
    {
        foreach (var entry in _equipmentDict) {
            var adventurer = entry.Key;
            var equipmentList = entry.Value;

            if (adventurer.Cards.Contains(card)) return adventurer;
            foreach (var equipment in equipmentList) {
                if (equipment == null) continue;
                if (equipment.Cards.Contains(card)) return adventurer;
            }
        }
        return null;
    }

    public void SetParty(List<AdventurerData> adventurers)
    {
        Adventurers.Clear();
        foreach (var a in adventurers) AddAdventurer(a);
    }

    public void AddAdventurer(AdventurerData adventurer)
    {
        Adventurers.Add(adventurer);
        _equipmentDict.Add(adventurer, new List<Equipment>(){null, null, null});
    }

    public List<Equipment> GetEquipment(AdventurerData adventurer)
    {
        return _equipmentDict[adventurer];
    }

    public void SetEquipment(AdventurerData adventurer, Equipment equipment, EquipmentSlot slot)
    {
        foreach (var entry in _equipmentDict) {
            for (int i = 0; i < entry.Value.Count; i++) {
                if (entry.Value[i] == equipment) entry.Value[i] = null;
            }
        }

        int index = SlotToIndex(slot);
        if (index == -1) return;

        _equipmentDict[adventurer][index] = equipment;
    }

    private int SlotToIndex(EquipmentSlot slot )
    {
        int index = -1;
        if (slot == EquipmentSlot.ORNAMENT) index = _ornamentIndex;
        if (slot == EquipmentSlot.ARMOR) index = _armorIndex;
        if (slot == EquipmentSlot.MAIN) index = _mainIndex;
        return index;
    }
}
