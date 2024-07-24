using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using System.Reflection;
using System.Threading.Tasks;

public enum Function { NONE, ATTACK, BLOCK, DRAW, HEAL, ADDCARDS, STATUS, TRIGGEREDEFFECT}

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
    [ConditionalField (nameof(Function), inverse:true, Function.NONE)] public float Amount = 50;
    [ConditionalField (nameof(Function), inverse:false, Function.ADDCARDS)] public CardData CardData;
    [ConditionalField (nameof(Function), inverse:false, Function.STATUS)] public StatusEffectData StatusEffectData;
    [ConditionalField (nameof(Function), inverse:false, Function.TRIGGEREDEFFECT)] public TriggeredEffectData TriggeredEffectData;
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
        List<CardFunctionData> functions = new List<CardFunctionData>(_cardFunctionData);
        var playData = new CardPlayData(CardGameManager.i.GetAdventurerObject(Owner), functions);

        foreach (var f in functions) { 
            if (f.Function == Function.ATTACK || f.Function == Function.STATUS) {
                playData.TargetTypes = new List<System.Type>() { typeof(EnemyObject) };
                break;
            } else if (f.Function == Function.HEAL) {
                playData.TargetTypes = new List<System.Type>() {typeof(AdventurerObject)};
            }
        }

        return playData;
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
