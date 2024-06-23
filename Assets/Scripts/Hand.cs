using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    const float CARD_ARC_ROT_SCALING_FACTOR = .7f;
    const float CARD_ARC_POS_SCALING_FACTOR_Y = 1.0f;
    const float CARD_ARC_POS_SCALING_FACTOR_X = 0.95f;

    public GameObject placeHolderCardPrefab;
    public List<CardObject> cards = new List<CardObject>();
    public int maxHandSize = 6;
    public Deck deck;
    public GameObject cardPrefab;
    public Transform playedCardDisplayTransform;

    //width of a card
    float evenHandSizeOffset = 100;

    void Update() {
    }

    public void ReorganizeHand() {
        foreach (CardObject card in cards) {
            card.localOriginPosition = Vector3.zero;
            card.localOriginRotation = Quaternion.identity;
        }

        float cardArcRotationFactor = cards.Count / 5f;
        Vector3 cardArcPositionFactorX = new Vector3(125, 0, 0);
        Vector3 cardArcPositionFactorY = new Vector3(0, -cardArcRotationFactor, 0);

        //iteration needs to be +1 for odd-numbered hands
        for (int i = 0; i < (cards.Count % 2 == 0 ? (cards.Count / 2) : (cards.Count - 1) / 2.0); i++) {
            //determine the magnitude of the offset
            int offsetMagnitude;
            if (cards.Count % 2 == 0) {
                offsetMagnitude = ((cards.Count / 2) - 1) - i;
            } else {
                offsetMagnitude = ((cards.Count - 1) / 2) - i;
            }

            //from the left
            cards[i].localOriginRotation = Quaternion.Euler(new Vector3(0, 0, cardArcRotationFactor * Mathf.Pow(offsetMagnitude, CARD_ARC_ROT_SCALING_FACTOR)));
            cards[i].localOriginPosition += cardArcPositionFactorY * Mathf.Pow(offsetMagnitude, CARD_ARC_POS_SCALING_FACTOR_Y);
            cards[i].localOriginPosition += -Utilities.Vec3Pow(cardArcPositionFactorX, CARD_ARC_POS_SCALING_FACTOR_X) * offsetMagnitude;

            //from the right
            cards[cards.Count - i - 1].localOriginRotation = Quaternion.Euler(new Vector3(0, 0, -cardArcRotationFactor * Mathf.Pow(offsetMagnitude, CARD_ARC_ROT_SCALING_FACTOR)));
            cards[cards.Count - i - 1].localOriginPosition += cardArcPositionFactorY * Mathf.Pow(offsetMagnitude, CARD_ARC_POS_SCALING_FACTOR_Y);
            cards[cards.Count - i - 1].localOriginPosition += Utilities.Vec3Pow(cardArcPositionFactorX, CARD_ARC_POS_SCALING_FACTOR_X) * (offsetMagnitude + (cards.Count % 2 == 0 ? 1 : 0));
        }

        //hands with even number of cards have a small offset
        if (cards.Count != 0) {
            if (cards.Count % 2 == 0) {
                transform.localPosition = new Vector3(-0.5f * evenHandSizeOffset, transform.localPosition.y, 0);
            } else {
                transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
            }
        }

        //set origins for cards so they can animate and whatnot
        foreach (CardObject card in cards) {
            card.transform.localPosition = card.localOriginPosition;
            card.transform.localRotation = card.localOriginRotation;
        }
    }

    public IEnumerator AddCard(CardData cardData) {
        //put placeholder card in hand
        GameObject newPlaceHolderCard = GameObject.Instantiate(placeHolderCardPrefab, transform);
        cards.Add(newPlaceHolderCard.GetComponent<CardObject>());
        ReorganizeHand();

        //initialize card
        GameObject newCard = GameObject.Instantiate(cardPrefab, transform);
        newCard.GetComponent<CardObject>().graphic.GetComponent<Image>().sprite = cardData.cardGraphic;
        newCard.GetComponent<CardObject>().cardData = cardData;
        newCard.GetComponent<CardObject>().cardData.cardObject = newCard.GetComponent<CardObject>();
        newCard.GetComponent<CardObject>().localOriginPosition = newPlaceHolderCard.GetComponent<CardObject>().localOriginPosition;
        newCard.GetComponent<CardObject>().localOriginRotation = newPlaceHolderCard.GetComponent<CardObject>().localOriginRotation;
        newCard.transform.position = deck.transform.position;

        //move new card to placeholder's position
        yield return StartCoroutine(Utilities.LerpObject(newCard, newPlaceHolderCard.transform, time: Utilities.OBJECT_LERP_TIME / 2));

        //replace placeholder with new card
        cards[cards.IndexOf(newPlaceHolderCard.GetComponent<CardObject>())] = newCard.GetComponent<CardObject>();
        Destroy(newPlaceHolderCard);

        //enable script
        newCard.GetComponent<CardObject>().enabled = true;

        //shake cards!
        foreach (CardObject card in cards) {
            StartCoroutine(card.Shake());
        }
        //need to wait for all shake coroutines to finish, but they are a fixed length? not sure if there's a cleaner way to wait
        //must be > Utilities.OBJECT_SHAKE_TIME because Utilities.ShakeObject() waits deltaTime so it isn't consistently the same length
        yield return new WaitForSeconds(Utilities.OBJECT_SHAKE_TIME * 1.2f);
    }

    public IEnumerator AddCard(CardObject cardObject) {
        //add card to list
        cards.Add(cardObject);
        ReorganizeHand();

        //shake cards!
        foreach (CardObject card in cards) {
            //StartCoroutine(card.Shake());
        }

        //need to wait for all shake coroutines to finish, but they are a fixed length? not sure if there's a cleaner way to wait
        yield return new WaitForSeconds(Utilities.OBJECT_SHAKE_TIME);
    }

    public void RemoveCard(CardObject cardObject) {
        StartCoroutine(RemoveCardCoroutine(cardObject));
    }

    IEnumerator RemoveCardCoroutine(CardObject cardObject) {
        cards.Remove(cardObject);
        ReorganizeHand();
        yield return null;
    }

    public IEnumerator PlayCard(CardObject cardObject) {
        RemoveCard(cardObject);
        yield return null;
    }

    public IEnumerator MoveToPlayedCardDisplay(CardObject cardObject) {
        RemoveCard(cardObject);
        cardObject.transform.SetParent(playedCardDisplayTransform);
        yield return StartCoroutine(Utilities.LerpObject(cardObject.gameObject, playedCardDisplayTransform));
    }

    public IEnumerator MoveFromPlayedCardDisplay(CardObject cardObject) {
        cardObject.transform.SetParent(transform);
        StartCoroutine(AddCard(cardObject));
        yield return null;
    }

}