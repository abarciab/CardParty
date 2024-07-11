using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Adventurer")]
public class AdventurerData : ScriptableObject
{
    public string Name;
    public Sprite portrait;
    [TextArea(3,10)] public string Description;
    public GameObject Adventurer;
    public List<CardData> Cards = new List<CardData>();

    public List<CardData> GetInnateCards(int total)
    {
        var list = new List<CardData>(Cards);
        while (list.Count < total) list.Add(Cards[Random.Range(0, Cards.Count)]);
        return list;
    }
}
