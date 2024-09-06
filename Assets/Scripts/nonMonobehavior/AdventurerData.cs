using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AdventurerRarity {Common, Uncommon, Rare};

[CreateAssetMenu(fileName = "Adventurer")]
public class AdventurerData : ScriptableObject
{
    public string Name;
    public Sprite portrait;
    [TextArea(3,10)] public string Description;
    public GameObject AdventurerPrefab;
    [DisplayInspector] public List<CardData> Cards = new List<CardData>();

    public int MaxHealth;

    public AdventurerStats Stats => PlayerInfo.Party.GetStats(this);
    public AdventurerRarity Rarity = AdventurerRarity.Common;
    public int Cost = 100;
    
    public List<CardData> GetInnateCards() {
        return Cards.OrderBy(x => x.Name).ToList();
    }

    public List<CardData> GetUniqueCards() {
        var newList = Cards;
        newList.Reverse();
        return newList.GetRange(0, 3).ToList();
    }

    public static AdventurerData GetAdventurer(int difficulty = -1) {
        if (difficulty == -1) difficulty = Constants.MAX_DIFFICULTY;

        AdventurerRarity rarity = AdventurerRarity.Common;
        float r = UnityEngine.Random.Range(0, 1);
        if (difficulty < 5) {
            if (r < 0.3) rarity = AdventurerRarity.Uncommon;
            else rarity = AdventurerRarity.Common;
        } else if (difficulty <= 9) {
            if (r < 0.1) rarity = AdventurerRarity.Rare;
            else if (r < 0.4) rarity = AdventurerRarity.Uncommon;
            else rarity = AdventurerRarity.Common;
        } else if (difficulty <= 12) {
            if (r < 0.2) rarity = AdventurerRarity.Rare;
            else if (r < 0.5) rarity = AdventurerRarity.Uncommon;
            else rarity = AdventurerRarity.Common;
        }

        foreach(AdventurerData adventurer in Resources.LoadAll("Adventurers", typeof(AdventurerData)).ToList().Shuffle()) {
            if (adventurer.Rarity == rarity) return adventurer;
        }

        return null;
    }
}