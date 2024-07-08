using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class CardObject: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public static int interactionTimeoutIdStream = 0;
    private List<int> interactionTimeoutIds = new List<int>();
    public GameObject Graphic;
    public CardData CardData;

    public Vector3 LocalOriginPosition;
    public Quaternion LocalOriginRotation;

    private bool _isHover = false;
    private bool _isBeingdragged = false;
    private bool _isBeingPlayed = false;
    private bool _isShaking = false;
    private bool _isInteractable = true;
    private IEnumerator _currPlayedCardCoroutine;
    public CardObject CurrPlaceHolderCard;
 
    void Update()
    {
        if (_isBeingdragged) {
            //Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePoint.z = 0;
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_isInteractable) {
            _isHover = true;
            if (!_isBeingPlayed && !_isShaking) {
                StartCoroutine(Shake());
            }
        }
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        _isHover = false;
    }
 
    public void OnPointerDown(PointerEventData eventData) {
        if (!_isBeingPlayed) {
            if (_isInteractable && _isHover && !_isShaking) {
                if (CardGameManager.i.CurrPlayedCard) {
                    CardGameManager.i.CurrPlayedCard.CardObject.CancelPlay();
                }
                StartDrag();
            }
        } else {
            //if card is already in the display
            if (CardGameManager.i.CurrPlayedCard) CancelPlay();
        }
    }
 
    public void OnPointerUp(PointerEventData eventData) {
        if (!_isInteractable || !_isBeingdragged) return;
        
        if (CardGameManager.i.HoveredPlayZone && !CardGameManager.i.CurrPlayedCard && CardGameManager.i.CurrCombatState == CombatState.PlayerTurn) {
            EndDrag();
            StartCoroutine(PlayCard());
        } else {
            transform.localPosition = LocalOriginPosition;
            EndDrag();
        }
    }

    void StartDrag() {
        _isBeingdragged = true;
        CardGameManager.i.DraggedCard = this;
        Graphic.GetComponent<Image>().raycastTarget = false;
    }

    void EndDrag() {
        _isBeingdragged = false;
        CardGameManager.i.DraggedCard = null;
        Graphic.GetComponent<Image>().raycastTarget = true;
    }

    public IEnumerator Shake() {
        _isShaking = true;
        yield return StartCoroutine(Utilities.ShakeObject(Graphic));
        _isShaking = false;
    }

    private IEnumerator PlayCard() {
        if (_isBeingPlayed || CardGameManager.i.CurrCombatState != CombatState.PlayerTurn) yield break;

        _isBeingPlayed = true;
        _isInteractable = false;
        EndDrag();

        CardGameManager.i.CardStartsPlay(this);
    }

    public void CancelPlay() {
        CardData.CancelPlay();
    }

    public void MoveFromDisplay() {
        _isBeingPlayed = false;
        _isInteractable = true;

        CardGameManager.i.MoveCardFromDisplay();
    }

    public void DisableInteractionForSeconds(float duration) {
        _isInteractable = false;

        int newInteractionTimeoutId = interactionTimeoutIdStream++;
        interactionTimeoutIds.Add(newInteractionTimeoutId);
        StartCoroutine(DisableInteractionForSeconds_Timeout(duration, newInteractionTimeoutId));
    }

    private IEnumerator DisableInteractionForSeconds_Timeout(float duration, int interactionTimeoutId) {
        yield return new WaitForSeconds(duration);
        interactionTimeoutIds.Remove(interactionTimeoutId);

        if (interactionTimeoutIds.Count == 0) _isInteractable = true;
    }
}