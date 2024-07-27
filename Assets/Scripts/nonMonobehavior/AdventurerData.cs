using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public List<CardData> GetInnateCards() {
        return Cards.OrderBy(x => x.Name).ToList();
    }

    public List<CardData> GetUniqueCards() {
        var newList = Cards;
        newList.Reverse();
        return newList.GetRange(0, 3).ToList();
    }
}