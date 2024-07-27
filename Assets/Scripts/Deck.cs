using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private GameObject _drawPileObj;
    [SerializeField] private GameObject _discardPileObj;

    private List<CardInstance> _cards = new List<CardInstance>();
    private List<CardInstance> _discardPile = new List<CardInstance>();

    public void Initialize()
    {
        foreach (CardData data in PlayerInfo.Party.GetDeck()) {
            _cards.Add(new CardInstance(data));
        }
        _drawPileObj.SetActive(true);
        _discardPileObj.SetActive(false);
    }

    public void AddToDeck(CardInstance inst, int count = 1, bool random = true) {
        for (int i = 0; i < count; i++) {
            _cards.Insert(random ? Random.Range(0, _cards.Count) : 0, inst);
        }
    }

    public void AddToDiscard(CardInstance inst, int count = 1)
    {
        print("discard pile size: " + _discardPile.Count);
        print("adding to discard pile from play");
        _discardPileObj.SetActive(true);

        for (int i = 0; i < count; i++) {
            _discardPile.Add(inst);
        }
        print(_discardPile.Count);
    }

    public void Draw(int count = 1) {
        print("_deckSize: " + _cards.Count);
        print("_discardSize: " + _discardPile.Count);
        print("drawing " + count);
        List<CardInstance> tempList = new List<CardInstance>();
        for(int i = 0; i < count; i++) {
            if (_cards.Count == 0) ShuffleDiscardPileIntoDrawPile();
            if (_cards.Count == 0) break; //nothing left to draw idiot
            tempList.Add(_cards[0]);
            _cards.RemoveAt(0);
        }

        _drawPileObj.SetActive(_cards.Count > 0);
        _hand.AddCards(tempList);

        print("_deckSize: " + _cards.Count);
        print("_discardSize: " + _discardPile.Count);
    }

    private void ShuffleDiscardPileIntoDrawPile()
    {
        print("_deckSize: " + _cards.Count);
        print("_discardSize: " + _discardPile.Count);
        print("shuffling " + _discardPile.Count + " cards from discard into draw pile");
        _cards.AddRange(new List<CardInstance>(_discardPile.Shuffle()));
        _drawPileObj.SetActive(_cards.Count > 0);
        _discardPile = new List<CardInstance>();
        _discardPileObj.SetActive(_discardPile.Count > 0);
        print("_deckSize: " + _cards.Count);
        print("_discardSize: " + _discardPile.Count);
    }
}