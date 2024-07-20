using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cardListParent;

    [SerializeField] private GameObject _playableCardPrefab;
    [SerializeField] private List<CardObject> _cards = new List<CardObject>();
    [SerializeField] private int _maxHandSize = 6;
    [SerializeField] private Deck _deck;

    public void AddCards(List<CardData> newCards) {

        List<GameObject> placeHolderCards = new List<GameObject>();
        foreach(CardData card in newCards) {
            GameObject newCardCoord = Instantiate(_playableCardPrefab, _cardListParent);
            var cardController = newCardCoord.GetComponent<CardObject>();
            cardController.Initialize(card, this);
            _cards.Add(cardController);

            placeHolderCards.Add(newCardCoord);
        }

        DisableCardInteractionForSeconds(Utilities.OBJECT_LERP_TIME + Utilities.OBJECT_SHAKE_TIME * 1.2f);
    }

    public void AddCard(CardObject cardObject) {
        if (_cards.Contains(cardObject)) return;
        _cards.Add(cardObject);
    }

    public void DrawUntilFull() {
        if (_maxHandSize > _cards.Count) _deck.Draw(_maxHandSize - _cards.Count);
    }

    public void RemoveCard(CardObject cardObject) {
        _cards.Remove(cardObject);
    }

    public void MoveFromDisplay(CardObject cardObject) {
        cardObject.transform.SetParent(transform);
        AddCard(cardObject);
    }

    private void DisableCardInteractionForSeconds(float duration) { }

}