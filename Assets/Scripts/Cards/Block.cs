using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "CardData/Block")]
public class Block : CardData
{
    public override IEnumerator PlayCard() {
        List<System.Type> requiredTargets = new List<System.Type>() {typeof(Adventurer), typeof(Adventurer)};

        currSelectTargets = CardGameManager.i.SelectTargets(requiredTargets);
        yield return CardGameManager.i.StartCoroutine(currSelectTargets);

        CardGameManager.i.CancelPlayCard();

        Creature defender = ((List<Creature>)currSelectTargets.Current).Find(x => x.GetType() == typeof(Adventurer));
        Creature defendee = ((List<Creature>)currSelectTargets.Current).Find(x => x.GetType() == typeof(Adventurer) && x != defender);
        
        yield return CardGameManager.i.StartCoroutine(defender.Block(50, defendee));
    }

    public override IEnumerator CancelPlayCard() {
        CardGameManager.i.StopCoroutine(currSelectTargets);
        yield return null;
    }
}