using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

public enum Function { NONE, ATTACK, BLOCK }

[CreateAssetMenu(fileName = "CardData")]
public class CardData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [TextArea(3, 10)] public string Description;
    [SerializeField] private Function _function;
    [ConditionalField (nameof(_function), inverse:true, Function.NONE), SerializeField] private float _amount = 50;
    [SerializeField] private CardSpecialData _specialData;

    [Header("Other Behvaiors")]
    [SerializeField] private bool _exhaust;

    [HideInInspector] public CardObject CardObject = null;

    private IEnumerator _currCardCoroutine;
    private IEnumerator _currSelectTargets;

    private Adventurer _owner = null;

    public void Init(AdventurerData data) {
        _owner = data.Adventurer.GetComponent<Adventurer>();
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
                defender.TakeDamage(_amount);

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
                defendee.AddBlock(_amount);

                CardGameManager.i.CardEndsPlay(CardObject);
            }
            break;
        }

        //DoSpecial();

        yield return null;
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
        return "name: " + Name + "\nfunction: " + _function + "\nowner: " + _owner;
    }
}
