using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using System.Threading.Tasks;

public enum Function { NONE, ATTACK, BLOCK, DRAW, SPECIAL }

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
    [ConditionalField (nameof(Function), inverse:true, Function.NONE)] public CardSpecialData CardSpecialData;
    [ConditionalField (nameof(Function), false, Function.SPECIAL)] public List<System.Type> Targets;

    public CardFunctionData(Function newFunction, int newAmount, List<System.Type> newTargets, CardSpecialData newSpecialData = null) {
        Function = newFunction;
        Amount = newAmount;
        Targets = newTargets;
        CardSpecialData = newSpecialData;
    }
}

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Description;
    [SerializeField] private List<CardFunctionData> _cardFunctionData = new List<CardFunctionData>();

    [Header("Other Behaviours")]
    [SerializeField] private bool _targetAll;
    [SerializeField] private bool _exhaust;

    private IEnumerator _currCardCoroutine;
    private IEnumerator _currSelectTargets;

    private AdventurerObject _owner = null;

    public AdventurerData Owner => PlayerInfo.Party.GetOwner(this);

    public void Init(AdventurerData data) {
        _owner = data.AdventurerPrefab.GetComponent<AdventurerObject>();
    }

    public override bool Equals(object other)
    {
        var otherCard = other as CardData;
        if (otherCard == null) return false;
        return Sprite == otherCard.Sprite && otherCard.name == name;
    }

    public string GetMoveData()
    {
        List<string> output = new List<string>();
        foreach (CardFunctionData cardFunctionData in _cardFunctionData) {
            if (cardFunctionData.Function == Function.ATTACK) output.Add("Attack " + (_targetAll ? "all " : "") + Utilities.Parenthize(cardFunctionData.Amount));
            if (cardFunctionData.Function == Function.BLOCK) output.Add("Block " + Utilities.Parenthize(cardFunctionData.Amount));
            if (cardFunctionData.Function == Function.DRAW) output.Add("Draw  " + Utilities.Parenthize(cardFunctionData.Amount));
            output.AddRange(cardFunctionData.CardSpecialData.GetMoveData());
        }
        if (_exhaust) output.Add("Exhaust");
        return string.Join("\n", output);
    }

    public CardPlayData GetPlayData(AdventurerObject OwnerAdventurer) {
        List<CardFunctionData> res = new List<CardFunctionData>(_cardFunctionData);

        //fill in assumed values, e.g. attacks having one target that is an enemy
        for (int i = 0; i < res.Count; i++) {
            if (res[i].Function == Function.ATTACK) res[i].Targets = new List<System.Type>(){typeof(EnemyObject)};
        }

        return new CardPlayData(CardGameManager.i.GetAdventurerObject(Owner), res);
    }

    public void CancelPlay()
    {
        if (_currSelectTargets != null) _owner.StopCoroutine(_currSelectTargets);
        Debug.Log("can't cancel play from here, no reference to cardObj");
    }

    public override string ToString() {
        return "name: " + Name + "\nowner: " + Owner;
    }
}