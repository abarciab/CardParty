using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using System.Reflection;
using System.Threading.Tasks;

public enum Function { NONE, ATTACK, BLOCK, DRAW, HEAL, ADDCARDS, STATUS, TRIGGEREDEFFECT, THEVESSEL, ARCHMAGEPROT, WHEEL, REMOVESTATUS}
[Serializable]
public class CardPlayData {
    public AdventurerObject Owner;
    
    public List<CardFunctionData> CardFunctionData; 
    public List<System.Type> TargetTypes = new List<System.Type>();

    public CardPlayData(AdventurerObject newOwner, List<CardFunctionData> newCardFunctionData) {
        Owner = newOwner;
        CardFunctionData = newCardFunctionData;
        TargetTypes = new List<Type>();
    }
}

[Serializable]
public class CardFunctionData {
    public Function Function;
    [ConditionalField (nameof(Function), inverse:true, Function.NONE, Function.TRIGGEREDEFFECT)] public float Amount = 50;
    [ConditionalField (nameof(Function), inverse:false, Function.ADDCARDS)] public CardData CardData;
    [ConditionalField (nameof(Function), inverse:false, Function.STATUS, Function.THEVESSEL)] public StatusEffectData StatusEffectData;
    [ConditionalField (nameof(Function), inverse:false, Function.TRIGGEREDEFFECT), DisplayInspector] public TriggeredEffectData TriggeredEffectData;
    [ConditionalField (nameof(Function), inverse:false, Function.STATUS)] public bool TargetSelf = false;
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

    public CardInstance (CardData data, AdventurerData newOwner = null) {
        Name = data.Name;
        Sprite = data.Sprite;
        Description = data.Description;
        _cardFunctionData = data.CardFunctionData;
        _targetAll = data.TargetAll;
        _exhaust = data.Exhaust;
        CardData = data;

        if (newOwner == null) Owner = data.Owner;
        else Owner = newOwner;
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
            if (cardFunctionData.Function == Function.HEAL) output.Add("Heal  " + Utilities.Parenthize(cardFunctionData.Amount));
            if (cardFunctionData.Function == Function.ADDCARDS) output.Add("Add  " + (int) cardFunctionData.Amount + " " + cardFunctionData.CardData.Name + " to deck");
            if (cardFunctionData.Function == Function.STATUS) output.Add(cardFunctionData.StatusEffectData.Name + " " + Utilities.Parenthize(cardFunctionData.Amount));
            if (cardFunctionData.Function == Function.TRIGGEREDEFFECT) output.Add(cardFunctionData.TriggeredEffectData.GetMoveText());
            if (cardFunctionData.Function == Function.REMOVESTATUS) output.Add("Remove status effects");

            if (cardFunctionData.Function == Function.THEVESSEL) output.Add("aidan forgot what this does");
            if (cardFunctionData.Function == Function.ARCHMAGEPROT) output.Add("aidan forgot what this does");
            if (cardFunctionData.Function == Function.WHEEL) output.Add("aidan forgot what this does");
        }
        if (_exhaust) output.Add("Exhaust");
        return string.Join("\n", output);
    }

    public bool HasTargets() {
        return GetPlayData().TargetTypes.Count > 0;
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

    public CardPlayData GetPlayData(AdventurerObject OwnerAdventurer = null) {
        if (OwnerAdventurer == null) OwnerAdventurer = CardGameManager.i.GetOwnerAdventurer(this);

        List<CardFunctionData> functions = new List<CardFunctionData>(_cardFunctionData);
        var playData = new CardPlayData(CardGameManager.i.GetAdventurerObject(Owner), functions);

        playData.TargetTypes = CardData.GetTargets();

        return playData;
    }

}