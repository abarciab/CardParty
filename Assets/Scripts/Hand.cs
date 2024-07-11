using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    const float CARD_ARC_ROT_SCALING_FACTOR = .7f;
    const float CARD_ARC_POS_SCALING_FACTOR_Y = 1.0f;
    const float CARD_ARC_POS_SCALING_FACTOR_X = 0.95f;

    [SerializeField] private GameObject _placeHolderCardPrefab;
    [SerializeField] private List<CardObject> _cards = new List<CardObject>();
    [SerializeField] private int _maxHandSize = 6;
    [SerializeField] private Deck _deck;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _playedCardDisplayTransform;

    //width of a card
    float evenHandSizeOffset = 100;

    void Update() {
    }

    public void ReorganizeHand() {
        foreach (CardObject card in _cards) {
            card.LocalOriginPosition = Vector3.zero;
            card.LocalOriginRotation = Quaternion.identity;
        }

        float cardArcRotationFactor = _cards.Count / 5f;
        Vector3 cardArcPositionFactorX = new Vector3(125, 0, 0);
        Vector3 cardArcPositionFactorY = new Vector3(0, -cardArcRotationFactor, 0);

        //iteration needs to be +1 for odd-numbered hands
        for (int i = 0; i < (_cards.Count % 2 == 0 ? (_cards.Count / 2) : (_cards.Count - 1) / 2.0); i++) {
            //determine the magnitude of the offset
            int offsetMagnitude;
            if (_cards.Count % 2 == 0) {
                offsetMagnitude = ((_cards.Count / 2) - 1) - i;
            } else {
                offsetMagnitude = ((_cards.Count - 1) / 2) - i;
            }

            //from the left
            _cards[i].LocalOriginRotation = Quaternion.Euler(new Vector3(0, 0, cardArcRotationFactor * Mathf.Pow(offsetMagnitude, CARD_ARC_ROT_SCALING_FACTOR)));
            _cards[i].LocalOriginPosition += cardArcPositionFactorY * Mathf.Pow(offsetMagnitude, CARD_ARC_POS_SCALING_FACTOR_Y);
            _cards[i].LocalOriginPosition += -Utilities.Vec3Pow(cardArcPositionFactorX, CARD_ARC_POS_SCALING_FACTOR_X) * offsetMagnitude;

            //from the right
            _cards[_cards.Count - i - 1].LocalOriginRotation = Quaternion.Euler(new Vector3(0, 0, -cardArcRotationFactor * Mathf.Pow(offsetMagnitude, CARD_ARC_ROT_SCALING_FACTOR)));
            _cards[_cards.Count - i - 1].LocalOriginPosition += cardArcPositionFactorY * Mathf.Pow(offsetMagnitude, CARD_ARC_POS_SCALING_FACTOR_Y);
            _cards[_cards.Count - i - 1].LocalOriginPosition += Utilities.Vec3Pow(cardArcPositionFactorX, CARD_ARC_POS_SCALING_FACTOR_X) * (offsetMagnitude + (_cards.Count % 2 == 0 ? 1 : 0));
        }

        //hands with even number of cards have a small offset
        if (_cards.Count != 0) {
            if (_cards.Count % 2 == 0) {
                transform.localPosition = new Vector3(-0.5f * evenHandSizeOffset, transform.localPosition.y, 0);
            } else {
                transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
            }
        }

        //set origins for cards so they can animate and whatnot
        foreach (CardObject card in _cards) {
            card.transform.localPosition = card.LocalOriginPosition;
            card.transform.localRotation = card.LocalOriginRotation;
        }
    }

    public void AddCard(List<CardData> cardData) {
        //put placeholder cards in hand
        List<GameObject> placeHolderCards = new List<GameObject>();
        foreach(CardData c in cardData) {
            GameObject newPlaceHolderCard = GameObject.Instantiate(_placeHolderCardPrefab, transform);
            placeHolderCards.Add(newPlaceHolderCard);
            _cards.Add(newPlaceHolderCard.GetComponent<CardObject>());
        }
        ReorganizeHand();

        for (int i = 0; i < cardData.Count; i++) {
            //initialize card
            GameObject newCard = GameObject.Instantiate(_cardPrefab, transform);
            newCard.GetComponent<CardObject>().Graphic.GetComponent<Image>().sprite = cardData[i].CardGraphic;
            newCard.GetComponent<CardObject>().CardData = cardData[i];
            newCard.GetComponent<CardObject>().CardData.CardObject = newCard.GetComponent<CardObject>();
            newCard.GetComponent<CardObject>().LocalOriginPosition = placeHolderCards[i].GetComponent<CardObject>().LocalOriginPosition;
            newCard.GetComponent<CardObject>().LocalOriginRotation = placeHolderCards[i].GetComponent<CardObject>().LocalOriginRotation;
            newCard.transform.position = _deck.transform.position;

            StartCoroutine(ReplacePlaceHolderWithCard(newCard, placeHolderCards[i].GetComponent<CardObject>()));
        }

        DisableCardInteractionForSeconds(Utilities.OBJECT_LERP_TIME + Utilities.OBJECT_SHAKE_TIME * 1.2f);
    }

    IEnumerator ReplacePlaceHolderWithCard(GameObject newCard, CardObject placeHolder) {
        //move new card to placeholder's position
        yield return StartCoroutine(Utilities.LerpObject(newCard, placeHolder.transform, time: Utilities.OBJECT_LERP_TIME / 2));

        //replace placeholder with new card
        _cards[_cards.IndexOf(placeHolder.GetComponent<CardObject>())] = newCard.GetComponent<CardObject>();
        Destroy(placeHolder.gameObject);

        //enable script
        newCard.GetComponent<CardObject>().enabled = true;

        //shake :)
        StartCoroutine(newCard.GetComponent<CardObject>().Shake());
    }

    public IEnumerator AddCard(CardObject cardObject) {
        //add card to list
        _cards.Add(cardObject);
        ReorganizeHand();

        foreach (CardObject card in _cards) {
            StartCoroutine(card.Shake());
        }

        //need to wait for all shake coroutines to finish, but they are a fixed length? not sure if there's a cleaner way to wait
        yield return new WaitForSeconds(Utilities.OBJECT_SHAKE_TIME);
    }

    public void DrawUntilFull() {
        if (_maxHandSize > _cards.Count) _deck.Draw(_maxHandSize - _cards.Count);
    }

    void RemoveCard(CardObject cardObject) {
        _cards.Remove(cardObject);
        ReorganizeHand();
    }

    public IEnumerator PlayCard(CardObject cardObject) {
        RemoveCard(cardObject);
        yield return null;
    }

    public void MoveToDisplayAndPlay(CardObject cardObject) {
        StartCoroutine(MoveToDisplayAndPlay_Coroutine(cardObject));
    }

    private IEnumerator MoveToDisplayAndPlay_Coroutine(CardObject cardObject) {
        RemoveCard(cardObject);
        
        cardObject.transform.SetParent(_playedCardDisplayTransform);
        yield return StartCoroutine(Utilities.LerpObject(cardObject.gameObject, _playedCardDisplayTransform));

        CardGameManager.i.CardPlayFunction(cardObject, cardObject.CardData.GetPlayData());
    }

    public void MoveFromDisplay(CardObject cardObject) {
        StartCoroutine(MoveFromDisplay_Coroutine(cardObject));
    }

    private IEnumerator MoveFromDisplay_Coroutine(CardObject cardObject) {
        cardObject.transform.SetParent(transform);
        yield return StartCoroutine(AddCard(cardObject));
    }

    private void DisableCardInteractionForSeconds(float duration) {
        foreach (CardObject cardObject in _cards) {
            cardObject.DisableInteractionForSeconds(duration);
        }
    }

}