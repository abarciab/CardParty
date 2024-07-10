using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Adventurer")]
public class AdventurerData : ScriptableObject
{
    public string Name;
    public Sprite portrait;
    [TextArea(3,10)] public string Description;
    public GameObject Adventurer;
    public List<CardData> Cards = new List<CardData>();
}
