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
    public GameObject Adventurer;
    public List<CardData> Cards = new List<CardData>();
    public int MaxHealth;

    public List<CardData> GetInnateCards(int total)
    {
        var list = new List<CardData>(Cards);
        while (list.Count < total) {
            foreach (var card in Cards) {
                if (list.Count < total) list.Add(card);
            }
        }
        list = list.OrderBy(x => x.Name).ToList();

        return list;
    }
}
