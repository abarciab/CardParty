using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

public enum Function { NONE, ATTACK, BLOCK, SPECIAL }

public class CardPlayData {
    public AdventurerData Owner;
    public Function Function;
    public float Amount;
    public Action SpecialData;

    public CardPlayData(AdventurerData newOwner, Function newFunction, int newAmount, Action newSpecialData = null) {
        Owner = newOwner;
        Function = newFunction;
        Amount = newAmount;
        SpecialData = newSpecialData;
    }
}

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    public string _name = "";
    public CardObject CardObject = null;
    public Sprite CardGraphic;
    [SerializeField] public Function _function;
    /*[ConditionalField (_function == Function.ATTACK)] */float Damage = 50;
    /*[ConditionalField (_function == Function.BLOCK)] */float Block = 50;
    /*[ConditionalField (_function == Function.SPECIAL)] */float SpecialData;

    private IEnumerator _currCardCoroutine;
    private IEnumerator _currSelectTargets;

    private Adventurer _owner = null;

    public void Init(AdventurerData data) {
        _owner = data.Adventurer.GetComponent<Adventurer>();
    }

    public void Play() {
        _currCardCoroutine = Play_Coroutine();
        CardObject.StartCoroutine(_currCardCoroutine);
    }

    private IEnumerator Play_Coroutine() {
        switch (_function) {
            case Function.ATTACK: {
                List<System.Type> requiredTargets = new List<System.Type>() {typeof(Enemy)};

                // wait until valid targets have been selected
                _currSelectTargets = CardGameManager.i.SelectTargets(requiredTargets);
                yield return CardGameManager.i.StartCoroutine(_currSelectTargets);

                Creature defender = ((List<Creature>)_currSelectTargets.Current).Find(x => x.GetType() == typeof(Enemy));

                yield return _owner.StartCoroutine(Utilities.LerpToAndBack(_owner.gameObject, defender.transform.position));
                defender.TakeDamage(Damage);

                CardGameManager.i.CardEndsPlay(CardObject);
            }
            break;
                
            case Function.BLOCK: {
                List<System.Type> requiredTargets = new List<System.Type>() {typeof(Adventurer)};

                // wait until valid targets have been selected
                _currSelectTargets = CardGameManager.i.SelectTargets(requiredTargets);
                yield return CardGameManager.i.StartCoroutine(_currSelectTargets);

                Creature defendee = ((List<Creature>)_currSelectTargets.Current).Find(x => x.GetType() == typeof(Adventurer));

                yield return _owner.StartCoroutine(Utilities.LerpToAndBack(_owner.gameObject, defendee.transform.position));
                defendee.AddBlock(Damage);

                CardGameManager.i.CardEndsPlay(CardObject);
            }
            break;
            
            case Function.SPECIAL: {
            }
            break;
        }

        yield return null;
    public override bool Equals(object other)
    {
        var otherCard = other as CardData;
        if (otherCard == null) return false;
        return string.Equals(ToString(), other.ToString());
    }

    public string GetMoveData()
    {
        List<string> output = new List<string>();
        if (_function == Function.ATTACK) output.Add("Attack " + Utilities.Parenthize(_amount));
        if (_function == Function.BLOCK) output.Add("Block " + Utilities.Parenthize(_amount));
        output.AddRange(_specialData.GetMoveData());
        if (_exhaust) output.Add("Exhaust");
        return string.Join("\n", output);
    }

    public CardPlayData GetPlayData() {
        if (_function == Function.SPECIAL) {
            return new CardPlayData(PlayerInfo.Party.GetOwner(this), _function, 0);
        }

        return new CardPlayData(PlayerInfo.Party.GetOwner(this), _function, 50);
    }

    public void CancelPlay() {
        _owner.StartCoroutine(CancelPlay_Coroutine());
    }

    private IEnumerator CancelPlay_Coroutine() {
        yield return null;

        switch (_function) {
            case Function.ATTACK:
            case Function.BLOCK:
                if (_currSelectTargets != null) _owner.StopCoroutine(_currSelectTargets);
                    break;
        }

        CardObject.MoveFromDisplay();
    }

    public override string ToString() {
        return "name: " + _name + "\nfunction: " + _function + "\nowner: " + _owner;
    }
}

public enum Function {
    ATTACK,
    BLOCK,
    SPECIAL
}