using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using System.Reflection;
using System.Threading.Tasks;

public enum Function { NONE, ATTACK, BLOCK, DRAW, ADDCARDS, STATUS, TRIGGEREDEFFECT}

[Serializable]
public class CardPlayData {
    public AdventurerObject Owner;
    
    public List<CardFunctionData> CardFunctionData;

    public CardPlayData(AdventurerObject newOwner, List<CardFunctionData> newCardFunctionData) {
        Owner = newOwner;
        CardFunctionData = newCardFunctionData;
    }
}

[Serializable]
public class CardFunctionData {
    public Function Function;
    [ConditionalField (nameof(Function), inverse:true, Function.NONE)] public float Amount = 50;
    [ConditionalField (nameof(Function), inverse:false, Function.ADDCARDS)] public CardInstance CardInstance;
    [ConditionalField (nameof(Function), inverse:false, Function.STATUS)] public StatusEffectData StatusEffectData;
    [ConditionalField (nameof(Function), inverse:false, Function.TRIGGEREDEFFECT)] public TriggeredEffectData TriggeredEffectData;
    public List<System.Type> Targets = new List<System.Type>();
}

[Serializable]
public class CardInstance
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Description;
    [SerializeField] private List<CardFunctionData> _cardFunctionData = new List<CardFunctionData>();

    [Header("Other Behaviours")]
    [SerializeField] private bool _targetAll;
    [SerializeField] private bool _exhaust;
    public AdventurerData Owner;
    public CardData CardData;

    public CardInstance (CardData data) {
        Name = data.Name;
        Sprite = data.Sprite;
        Description = data.Description;
        _cardFunctionData = data.CardFunctionData;
        _targetAll = data.TargetAll;
        _exhaust = data.Exhaust;
        Owner = data.Owner;
        CardData = data;
    }

    public CardInstance Copy() {
        return (CardInstance)this.MemberwiseClone();
    }

    public override bool Equals(object other)
    {
        var otherCard = other as CardInstance;
        if (otherCard == null) return false;
        return Sprite == otherCard.Sprite && otherCard.Name == Name;
    }

    public string GetMoveData()
    {
        List<string> output = new List<string>();
        foreach (CardFunctionData cardFunctionData in _cardFunctionData) {
            if (cardFunctionData.Function == Function.ATTACK) output.Add("Attack " + (_targetAll ? "all " : "") + Utilities.Parenthize(cardFunctionData.Amount));
            if (cardFunctionData.Function == Function.BLOCK) output.Add("Block " + Utilities.Parenthize(cardFunctionData.Amount));
            if (cardFunctionData.Function == Function.DRAW) output.Add("Draw  " + Utilities.Parenthize(cardFunctionData.Amount));
        }
        if (_exhaust) output.Add("Exhaust");
        return string.Join("\n", output);
    }

    public CardPlayData GetPlayData(AdventurerObject OwnerAdventurer) {
        List<CardFunctionData> res = new List<CardFunctionData>(_cardFunctionData);

        //fill in assumed values, e.g. attacks having one target that is an enemy
        for (int i = 0; i < res.Count; i++) {
            if (res[i].Function == Function.ATTACK || res[i].Function == Function.STATUS) res[i].Targets = new List<System.Type>(){typeof(EnemyObject)};
        }

        return new CardPlayData(CardGameManager.i.GetAdventurerObject(Owner), res);
    }

    public override string ToString() {
        return "name: " + Name + "\nowner: " + Owner;
    }

    public static bool operator ==(CardInstance a, CardInstance b) {
        foreach (PropertyInfo propertyInfo in typeof(CardInstance).GetProperties()) {
            if (propertyInfo == typeof(CardInstance).GetProperty("Owner")) continue; //we don't care about this one
            if (propertyInfo.GetValue(a, null) != propertyInfo.GetValue(b, null)) return false;
        }
        return true;
    }

    public static bool operator !=(CardInstance a, CardInstance b) {
        foreach (PropertyInfo propertyInfo in typeof(CardInstance).GetProperties()) {
            if (propertyInfo == typeof(CardInstance).GetProperty("Owner")) continue; //we don't care about this one
            if (propertyInfo.GetValue(a, null) != propertyInfo.GetValue(b, null)) return true;
        }
        return false;
    }

}
