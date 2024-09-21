using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Party
{
    public List<AdventurerData> Adventurers = new List<AdventurerData>();

    private Dictionary<AdventurerData, List<EquipmentData>> _equipmentDict = new Dictionary<AdventurerData, List<EquipmentData>>();
    private Dictionary<AdventurerData, AdventurerStats> _statsDict = new Dictionary<AdventurerData, AdventurerStats>();

    private const int _ornamentIndex = 0;
    private const int _armorIndex = 1;
    private const int _mainIndex = 2;

    public override string ToString()
    {
        var output = "";
        foreach (var Adventurer in Adventurers) {
            output += Adventurer + ": " + string.Join(", ", _equipmentDict[Adventurer]) + "\n";
        }
        return output;
    }

    public void KillRandomAdventurer()
    {
        FireAdventurer(Adventurers[Random.Range(0, Adventurers.Count)]);
    }

    public void FireAdventurer(AdventurerData adventuer)
    {
        _equipmentDict.Remove(adventuer);
        _statsDict.Remove(adventuer);
        Adventurers.Remove(adventuer);
    }

    public void HealAllAdventurers(float percent = -1)
    {
        if (percent == -1) percent = 1;
        foreach (var stat in _statsDict.Values) stat.CurrentHealth += (int)(percent * stat.MaxHealth);
    }

    public AdventurerStats GetStats(AdventurerData adventurer) {
        return _statsDict[adventurer];
    }

    public List<CardData> GetDeckSorted()
    {
        var deck = new List<CardData>();

        foreach (var entry in _equipmentDict) {
            var adventurer = entry.Key;
            var equipmentList = entry.Value;

            deck.AddRange(adventurer.GetInnateCards());
            foreach (var equipment in equipmentList) {
                if (equipment != null) deck.AddRange(equipment.Cards);
            }
        }
        return deck.ToList();
    }

    public List<CardData> GetDeckShuffled()
    {
        var deck = GetDeckSorted();
        return deck.Shuffle().ToList();
    }

    public List<EquipmentData> GetAllEquippedItems()
    {
        var list = new List<EquipmentData>();
        foreach (var equipmentList in _equipmentDict.Values) {
            foreach (var e in equipmentList) if (e != null) list.Add(e); 
        }
        return list;
    }

    public EquipmentData GetEquippedEquipment() {
        List<EquipmentData> res = GetAllEquippedItems();

        return res[Random.Range(0, res.Count)];
    }

    public EquipmentData GetCurrentEquipment(AdventurerData adventurer, EquipmentSlot slot)
    {
        int index = SlotToIndex(slot);
        return _equipmentDict[adventurer][index];
    }

    public AdventurerData GetOwner(EquipmentData equipment)
    {
        foreach (var info in _equipmentDict) {
            if (info.Value.Contains(equipment)) return info.Key;
        }
        return null;
    }

    public AdventurerData GetOwner(CardData data) {
        foreach (var adventurer in Adventurers) {
            if (adventurer.Cards.Contains(data)) return adventurer;
            foreach (var e in _equipmentDict[adventurer]) {
                if (e && e.Cards.Contains(data)) return adventurer;
            }
        }

        //Debug.Log("didn't find adventurerOwner for: " + data.Name);
        return null;
    }

    public void SetParty(List<AdventurerData> adventurers)
    {
        ClearData();
        foreach (var a in adventurers) AddAdventurer(a);
    }

    private void ClearData()
    {
        Adventurers.Clear();
        _equipmentDict = new Dictionary<AdventurerData, List<EquipmentData>>();
        _statsDict = new Dictionary<AdventurerData, AdventurerStats>();
    }

    public void DamageSingle(int amount) {
        var stat = _statsDict.Values.ToList()[Random.Range(0, _statsDict.Values.Count)];
        stat.CurrentHealth = Mathf.Max(1, stat.CurrentHealth - amount);
    }

    public void DamageAll(int amount)
    {
        foreach (var stat in _statsDict.Values) stat.CurrentHealth = Mathf.Max(1, stat.CurrentHealth - amount);
    }

    public void AddAdventurer(AdventurerData adventurer = null, int difficulty = -1)
    {
        if (!adventurer) {
            if (difficulty == -1) {
                difficulty = Constants.MAX_DIFFICULTY;
            }

            adventurer = AdventurerData.GetAdventurer(difficulty);
            while (Adventurers.Contains(adventurer)) {
                adventurer = AdventurerData.GetAdventurer(difficulty);
            }
        } else {
            if (Adventurers.Contains(adventurer)) return;
        }

        Adventurers.Add(adventurer);
        _equipmentDict.Add(adventurer, new List<EquipmentData>(){null, null, null});
        _statsDict.Add(adventurer, new AdventurerStats(adventurer.MaxHealth));
    }

    public void SwapAdventurer(AdventurerData oldAdventurer = null, AdventurerData newAdventurer = null, int difficulty = -1) {
        if (difficulty == -1) difficulty = Constants.MAX_DIFFICULTY;

        if (!newAdventurer) newAdventurer = AdventurerData.GetAdventurer(difficulty);
        if (!oldAdventurer) oldAdventurer = Adventurers[Random.Range(0, Adventurers.Count)];

        AdventurerData temp = oldAdventurer;
        oldAdventurer = newAdventurer;
        newAdventurer = temp;
    }

    public List<EquipmentData> GetEquipment(AdventurerData adventurer)
    {
        return _equipmentDict[adventurer];
    }

    public void SetEquipment(AdventurerData adventurer, EquipmentData equipment, EquipmentSlot slot)
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

    public float GetPartyHealthPercent() {
        float val = 0;
        foreach (AdventurerStats stats in _statsDict.Values) {
            val += stats.CurrentHealth / stats.MaxHealth;
        }
        return val / Adventurers.Count;
    }

}
