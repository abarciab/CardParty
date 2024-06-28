using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardObject: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject graphic;
    public Vector3 localOriginPosition;
    public Quaternion localOriginRotation;
    public CardData cardData;

    //drag and drop

    bool isHover = false;
    bool dragging = false;
    bool playing = false;
    public bool isShaking = false;
    public bool interactable = true;
    IEnumerator currPlayedCardCoroutine;
    public CardObject currPlaceHolderCard;
 
    void Update()
    {
        if (dragging) {
            //Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePoint.z = 0;
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (interactable) {
            isHover = true;
            if (!playing && !isShaking) {
                StartCoroutine(Shake());
            }
        }
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        isHover = false;
    }
 
    public void OnPointerDown(PointerEventData eventData) {
        if (!playing) {
            if (interactable && isHover && !isShaking) {
                if (CardGameManager.i.currPlayedCard) {
                    StartCoroutine(CardGameManager.i.currPlayedCard.cardObject.CancelPlayCard());
                }
                StartDrag();
            }
        } else {
            //if card is already in the display
            if (CardGameManager.i.currPlayedCard) StartCoroutine(CancelPlayCard());
        }
    }
 
    public void OnPointerUp(PointerEventData eventData) {
        if (interactable) {
            if (dragging) {
                if (CardGameManager.i.hoveredPlayZone && !CardGameManager.i.currPlayedCard && CardGameManager.i.currCombatState == CombatState.PlayerTurn) {
                    EndDrag();
                    StartCoroutine(PlayCard());
                } else {
                    transform.localPosition = localOriginPosition;
                    EndDrag();
                }
            }
        }
    }

    void StartDrag() {
        dragging = true;
        CardGameManager.i.draggedCard = this;
        graphic.GetComponent<Image>().raycastTarget = false;
    }

    void EndDrag() {
        dragging = false;
        CardGameManager.i.draggedCard = null;
        graphic.GetComponent<Image>().raycastTarget = true;
    }

    public IEnumerator Shake() {
        isShaking = true;
        yield return StartCoroutine(Utilities.ShakeObject(graphic));
        isShaking = false;
    }

    IEnumerator PlayCard() {
        if (!playing && CardGameManager.i.currCombatState == CombatState.PlayerTurn) {

            //print("just called play card on: " + gameObject.name);

            playing = true;
            interactable = false;

            // move the card off to the side of the screen while it evaluates
            EndDrag();
            yield return StartCoroutine(CardGameManager.i.hand.MoveToPlayedCardDisplay(this));

            //effect of the card being played - starts targeting, performs play effect
            // print("just set currPlayedCard equal to the cardData for: " + gameObject.name);
            CardGameManager.i.currPlayedCard = cardData;
            // print("value of currPlayedCard.cardData.cardObject: " + CardGameManager.i.currPlayedCard.cardObject.gameObject.name);
            cardData.currCardCoroutine = cardData.PlayCard();
            yield return StartCoroutine(CardGameManager.i.currPlayedCard.PlayCard());

            //handling the card object
            yield return StartCoroutine(CardGameManager.i.hand.PlayCard(this));

            //combat manager does play card stuff
            CardGameManager.i.PlayCard();

            Destroy(gameObject);
        }
    }

    IEnumerator CancelPlayCard() {
        yield return StartCoroutine(cardData.CancelPlayCard());
        yield return StartCoroutine(CardGameManager.i.CancelPlayCard());

        playing = false;
        interactable = true;
    }


}