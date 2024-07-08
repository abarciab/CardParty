using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private List<CardData> _cards = new List<CardData>();

    public void AddCard(CardData newCard) {
        _cards.Insert(Random.Range(0, _cards.Count), newCard);
    }

    public void Draw(int count = 1) {
        List<CardData> tempList = new List<CardData>();
        for(int i = 0; i < count; i++) {
            if (_cards.Count == 0) break;
            tempList.Add(_cards[0]);
            _cards.RemoveAt(0);
        }

        _hand.AddCard(tempList);
    }
}