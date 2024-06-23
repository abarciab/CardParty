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
    bool isShaking = false;
    bool interactable = true;
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

    //drag and drop
 
    public void OnPointerExit(PointerEventData eventData) {
        isHover = false;
    }
 
    public void OnPointerDown(PointerEventData eventData) {
        if (!playing) {
            if (interactable && isHover && !isShaking) StartDrag();
        } else if (CardGameManager.i.currCombatState == CombatState.SelectTargets) {
            StartCoroutine(CancelPlayCard());
        }
    }
 
    public void OnPointerUp(PointerEventData eventData) {
        if (interactable) {
            if (dragging) {
                if (CardGameManager.i.hoveredPlayZone && CardGameManager.i.currCombatState == CombatState.PlayerTurn) {
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
        yield return null;
        isShaking = false;
    }

    IEnumerator PlayCard() {
        if (!playing && CardGameManager.i.currCombatState == CombatState.PlayerTurn) {

            playing = true;
            interactable = false;

            // move the card off to the side of the screen while it evaluates
            EndDrag();
            yield return StartCoroutine(CardGameManager.i.hand.MoveToPlayedCardDisplay(this));

            //effect of the card being played - starts targeting, performs play effect
            CardGameManager.i.currPlayedCard = cardData;
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