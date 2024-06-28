using MyBox;
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

    /*
    private void PlayCard(){
        base.Play()
        CardGameManager.PassInCardMoveData({Adventurer, Enemy}, TYPE.ATTACK, Damage)
    }
    */

    /*
    
    string name
    private ID owner (character, equipment)
    private enum basicFunction {ATTACK, BLOCK, ATTACKBLOCK, SPECIAL}
    [conditionField (if ATTACK)] float Damage
    [conditionField (if ATTACK)] float poisonDamage
    [conditionField (if BLOCK)] float blockAmount
    [conditionField (if SPECIAL)] SPECIALATTACKDATA specialData
     
    public void PlayCard(){
        DoParentStuff()
        
        if (type == ATTACK) ATTACK()
        if (type == BLOCK) BLOCK()
        if (type == ATTACKBLOCK) {BLOCK(); ATTACK();}
        if (type == SPECIAL) specialData.Activate()
    }
     
    */


    /*
    
    public enum Type {BUFF, STUN, DEBUF}

    public void ACTIVATE(){
        if (type == BUFF) BUFF()
        if (type == STUN) STUN()
        if (type == DEBUF) DEBUF()
    }

    private void STUN() =>CardGameManager.PassInCardMoveData({Adventurer, Enemy}, TYPE.STUN, Amount)


    */


    public override IEnumerator CancelPlayCard() {
        if (currSelectTargets != null) CardGameManager.i.StopCoroutine(currSelectTargets);
        yield return null;
    }
}