using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Adventurer")]
public class AdventurerData : ScriptableObject
{
    public string Name;
    public GameObject Adventurer;
    public List<CardData> Cards = new List<CardData>();
}
