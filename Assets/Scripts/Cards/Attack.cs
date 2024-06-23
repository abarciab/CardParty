using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "CardData/Attack")]
public class Attack : CardData
{
    public override IEnumerator PlayCard() {
        base.PlayCard();

        List<System.Type> requiredTargets = new List<System.Type>() {typeof(Adventurer), typeof(Enemy)};

        currSelectTargets = CardGameManager.i.SelectTargets(requiredTargets);
        yield return CardGameManager.i.StartCoroutine(currSelectTargets);

        CardGameManager.i.CancelPlayCard();

        Creature attacker = ((List<Creature>)currSelectTargets.Current).Find(x => x.GetType() == typeof(Adventurer));
        Creature defender = ((List<Creature>)currSelectTargets.Current).Find(x => x.GetType() == typeof(Enemy));

        yield return CardGameManager.i.StartCoroutine(attacker.Attack(50, defender));
    }

    public override IEnumerator CancelPlayCard() {
        CardGameManager.i.StopCoroutine(currSelectTargets);
        yield return null;
    }
}