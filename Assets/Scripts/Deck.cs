using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private List<CardData> _cards = new List<CardData>();
    [SerializeField] private List<CardData> _discardPile = new List<CardData>();

    public void Initialize()
    {
        var cards = PlayerInfo.Party.GetDeck();
        _cards = new List<CardData>(cards.Shuffle());
    }

    public void AddToDiscard(CardData data)
    {
        _discardPile.Add(data);
    }

    public void Draw(int count = 1) {
        List<CardData> tempList = new List<CardData>();
        for(int i = 0; i < count; i++) {
            if (_cards.Count == 0) ShuffleDiscardPileIntoDrawPile();
            tempList.Add(_cards[0]);
            _cards.RemoveAt(0);
        }

        _hand.AddCards(tempList);
    }

    private void ShuffleDiscardPileIntoDrawPile()
    {
        _cards.AddRange(new List<CardData>(_discardPile.Shuffle()));
        print("Shuffling!");
    }
}