using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyBox;

public class CardObject: MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _hoveredWidth = 550;
    [SerializeField] private Vector2 _randomRotRange = new Vector2(-3, 3);
    [SerializeField] private float _hoverJumpUpDist = 35;
    [SerializeField, Tag] private string _playZoneTag;

    [Header("References")]
    [SerializeField] private SelectableItem _selectable;
    [SerializeField] private PlayableCardDisplay _display;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _shakingAnimString = "shaking";

    [HideInInspector] public CardData CardData { get; private set; }

    private bool _isBeingdragged = false;
    private float _startY;
    private float _originalWidth;
    private int _handSiblingIndex;

    private RectTransform _rTransform;
    private Hand _handController;
    private Transform _handGridParent;

    private void Start()
    {
        _startY = _display.transform.localPosition.y;
        _rTransform = GetComponent<RectTransform>();
        _originalWidth = _rTransform.sizeDelta.x;

        _selectable.OnHover.AddListener(StartHover);
        _selectable.OnEndHover.AddListener(EndHover);
        _selectable.OnSelect.AddListener(StartDrag);
    }

    private void EndHover()
    {
        _animator.SetBool(_shakingAnimString, false);
        SetWidth(_originalWidth);
        SetY(_startY);
    }

    private void StartHover()
    {
        _animator.SetBool(_shakingAnimString, true);
        SetWidth(_hoveredWidth);
        SetY(_startY + _hoverJumpUpDist);
    }

    private void SetY(float y)
    {
        var pos = _display.transform.localPosition;
        pos.y = y;
        _display.transform.localPosition = pos;
    }

    private void SetWidth(float width)
    {
        var size = _rTransform.sizeDelta;
        size.x = width;
        _rTransform.sizeDelta = size;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }
 
    void Update()
    {
        if (_isBeingdragged) UpdateDrag(); 
    }

    private void UpdateDrag()
    {
        transform.position = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) EndDrag();
    }

    public void Initialize(CardData data, Hand handController)
    {
        CardData = data;
        _handController = handController;
        _display.Initialize(data);
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.forward * Random.Range(_randomRotRange.x, _randomRotRange.y);
        _handGridParent = transform.parent;
    }

    void StartDrag()
    {
        EndHover();
        _handSiblingIndex = transform.GetSiblingIndex();
        _selectable.SetEnabled(false);

        _isBeingdragged = true;
        transform.SetParent(transform.parent.parent);
    }

    void EndDrag() {
        _isBeingdragged = false;

        if (IsCurrentlyInPlayZone()) PlayCard();
        else ReturnToHand();
    }

    private bool IsCurrentlyInPlayZone()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (var r in results) {
            if (r.gameObject.CompareTag(_playZoneTag)) return true;
        }
        return false;
    }

    private void ReturnToHand()
    {
        transform.SetParent(_handGridParent);
        transform.SetSiblingIndex(_handSiblingIndex);
        _selectable.SetEnabled(true);
    }

    private void PlayCard() {
        _handController.RemoveCard(this);
        _handController.MoveToDisplay(this);
        CardGameManager.i.PlayCard(this);
    }

    public void CancelPlay() {
        CardData.CancelPlay();
    }

    public void MoveFromDisplay() {
        CardGameManager.i.MoveCardFromDisplay();
    }
}