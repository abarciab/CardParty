using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class CardObject: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject graphic;
    public Vector3 localOriginPosition;
    public Quaternion localOriginRotation;
    public CardData cardData;

    //drag and drop

    bool isHover = false;
    bool isBeingdragged = false;
    bool isBeingPlayed = false;
    public bool isShaking = false;
    public bool isInteractable = true;
    private IEnumerator currPlayedCardCoroutine;
    public CardObject currPlaceHolderCard;
 
    void Update()
    {
        if (isBeingdragged) {
            //Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePoint.z = 0;
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (isInteractable) {
            isHover = true;
            if (!isBeingPlayed && !isShaking) {
                StartCoroutine(Shake());
            }
        }
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        isHover = false;
    }
 
    public void OnPointerDown(PointerEventData eventData) {
        if (!isBeingPlayed) {
            if (isInteractable && isHover && !isShaking) {
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
        if (!isInteractable || !isBeingdragged) return;
        
        if (CardGameManager.i.hoveredPlayZone && !CardGameManager.i.currPlayedCard && CardGameManager.i.currCombatState == CombatState.PlayerTurn) {
            EndDrag();
            StartCoroutine(PlayCard());
        } else {
            transform.localPosition = localOriginPosition;
            EndDrag();
        }
    }

    void StartDrag() {
        isBeingdragged = true;
        CardGameManager.i.draggedCard = this;
        graphic.GetComponent<Image>().raycastTarget = false;
    }

    void EndDrag() {
        isBeingdragged = false;
        CardGameManager.i.draggedCard = null;
        graphic.GetComponent<Image>().raycastTarget = true;
    }

    public IEnumerator Shake() {
        isShaking = true;
        yield return StartCoroutine(Utilities.ShakeObject(graphic));
        isShaking = false;
    }

    private IEnumerator PlayCard() {
        if (isBeingPlayed || CardGameManager.i.currCombatState != CombatState.PlayerTurn) yield break;

        //print("just called play card on: " + gameObject.name);

        isBeingPlayed = true;
        isInteractable = false;

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

    /*
    
    class2: 
    {
        SomeFunction()
        {
            doAllInternalStuff()
            Class1.PlayCard()
        }

    }


    public void PlayCard() => StartCoroutine(PlayCardInternal())

    private Ienumerator PlayCardInternal(float time = 1){
        yield return AnimateCard()
        PlayCardStep1()
    }
     
    PlayCardStep1(){
        gameManager.PlayCard()
    }
     */

    IEnumerator CancelPlayCard() {
        yield return StartCoroutine(cardData.CancelPlayCard());
        yield return StartCoroutine(CardGameManager.i.CancelPlayCard());

        isBeingPlayed = false;
        isInteractable = true;
    }


}