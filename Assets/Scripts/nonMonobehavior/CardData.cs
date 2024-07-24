using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using System.Reflection;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Description;
    public List<CardFunctionData> CardFunctionData = new List<CardFunctionData>();

    [Header("Other Behaviours")]
    public bool TargetAll;
    public bool Exhaust;
    public AdventurerData Owner => PlayerInfo.Party.GetOwner(this);

    public override bool Equals(object other)
    {
        var otherCard = other as CardData;
        if (otherCard == null) return false;
        return Sprite == otherCard.Sprite && otherCard.name == name;
    }

    public override string ToString() {
        return "name: " + Name + "\nowner: " + PlayerInfo.Party.GetOwner(this);
    }

    public static bool operator ==(CardData a, CardData b) {
        if (a is null || b is null) return false;
        foreach (PropertyInfo propertyInfo in typeof(CardData).GetProperties()) {
            if (propertyInfo.Name == "Owner") continue; // we don't care about owner, and this is also not logically cohesive with Owner initial value
            if (propertyInfo.GetValue(a, null) != propertyInfo.GetValue(b, null)) return false;
        }
        return true;
    }

    public static bool operator !=(CardData a, CardData b) {
        foreach (PropertyInfo propertyInfo in typeof(CardData).GetProperties()) {
            if (propertyInfo.Name == "Owner") continue; // we don't care about owner, and this is also not logically cohesive with Owner initial value
            if (propertyInfo.GetValue(a, null) != propertyInfo.GetValue(b, null)) return true;
        }
        return false;
    }
}