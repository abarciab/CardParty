using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> cards = new List<CardData>();

    public void Awake() {
        // for (int i = 0; i < 50; i++) {
        //     cards.Add(GameManager.inst.cardPool[Random.Range(0, GameManager.inst.cardPool.Count)]);
        // }
    }

    public IEnumerator AddCard(CardData newCard) {
        //anim here
        cards.Insert(Random.Range(0, cards.Count), newCard);
        yield return 0;
    }

    public IEnumerator DrawCard() {
        yield return StartCoroutine(CardGameManager.i.hand.AddCard(cards[0]));

        cards.RemoveAt(0);
    }
}