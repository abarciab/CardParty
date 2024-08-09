using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor.ShaderGraph.Internal;
using System.Xml.Serialization;
using UnityEditorInternal;

public enum SelectableItemDataType { GRAPHIC, GAMEOBJECT, CANVASGROUP}

[System.Serializable]
public class SelectableItemData
{
    [HideInInspector] public string Name;
    [SerializeField] private SelectableItemDataType _type;

    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GRAPHIC), SerializeField] private Graphic _target;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GRAPHIC), SerializeField] private Color _normalColor = Color.white;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GRAPHIC), SerializeField] private Color _hoveredColor = Color.white;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GRAPHIC), SerializeField] private Color _selectedColor = Color.white;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GRAPHIC), SerializeField] private Color _disabledColor = Color.white;

    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GAMEOBJECT), SerializeField] private GameObject _obj;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GAMEOBJECT), SerializeField] private bool _normalState = false;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GAMEOBJECT), SerializeField] private bool _hoveredState = false;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GAMEOBJECT), SerializeField] private bool _selectedState = true;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.GAMEOBJECT), SerializeField] private bool _disabledState = false;


    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.CANVASGROUP), SerializeField] private CanvasGroup _group;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.CANVASGROUP), SerializeField, Range(0, 1)] private float _normalAlpha = 0.3f;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.CANVASGROUP), SerializeField, Range(0, 1)] private float _hoveredAlpha = 0.6f;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.CANVASGROUP), SerializeField, Range(0, 1)] private float _selectedAlpha = 1;
    [ConditionalField(nameof(_type), false, false, SelectableItemDataType.CANVASGROUP), SerializeField, Range(0, 1)] private float _disabledAlpha = 0.2f;

    private bool _isGraphic => _type == SelectableItemDataType.GRAPHIC;
    private bool _isGameObject => _type == SelectableItemDataType.GAMEOBJECT;
    private bool _isGroup => _type == SelectableItemDataType.CANVASGROUP;

    public void OnValidate()
    {
        if ((_isGraphic && !_target) || (_isGameObject && !_obj) || (_isGroup && !_group)) Initialize();
        else SetName();
    }

    private void SetName()
    {
        if (_isGraphic) Name = _target.gameObject.name;
        if (_isGameObject) Name = _obj.name;
        if (_isGroup) Name = _group.gameObject.name;
    }

    private void Initialize()
    {
        _normalColor = Color.white;
        _hoveredColor = Color.white;
        _selectedColor = Color.white;
        _disabledColor = Color.white;

        _normalState = false;
        _hoveredState = false;
        _selectedState = true;
        _disabledState = false;

        _normalAlpha = 0.3f;
        _hoveredAlpha = 0.6f;
        _selectedAlpha = 1;
        _disabledAlpha = 0.2f;
    }

    public void Update(bool selected, bool hovered, bool disabled)
    {
        if (disabled) Disable();
        else if (selected) Select();
        else if (hovered) Hover();
        else Deselect();
    }

    private void Select()
    {
        if (_isGraphic) _target.color = _selectedColor;
        if (_isGroup) _group.alpha = _selectedAlpha;
        if (_isGameObject) _obj.SetActive(_selectedState);
    }

    private void Hover()
    {
        if (_isGraphic) _target.color = _hoveredColor;
        if (_isGroup) _group.alpha = _hoveredAlpha;
        if (_isGameObject) _obj.SetActive(_hoveredState);
    }

    private void Deselect()
    {
        if (_isGraphic) _target.color = _normalColor;
        if (_isGroup) _group.alpha = _normalAlpha;
        if (_isGameObject) _obj.SetActive(_normalState);
    }

    private void Disable()
    {
        if (_isGraphic) _target.color = _disabledColor;
        if (_isGroup) _group.alpha = _disabledAlpha;
        if (_isGameObject) _obj.SetActive(_disabledState);
    }
}

public class SelectableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Behavior")]
    [SerializeField] private bool _selectOnClick = true;
    [SerializeField, ConditionalField(nameof(_selectOnClick))] private bool _toggleOnClick;
    [SerializeField, ConditionalField(nameof(_toggleOnClick))] private bool _deselectOnClick = false;
    [SerializeField] private bool _selectOnHover;
    [SerializeField, ConditionalField(nameof(_selectOnHover))] private bool _deselectOnExit = true;
    [SerializeField] private bool _hasHoverCooldown;
    [SerializeField, ConditionalField(nameof(_hasHoverCooldown))] private float _hoverCooldown = 0.05f;
    [SerializeField] private bool _deselectOnStart = true;

    [Header("data")]
    [SerializeField] private List<SelectableItemData> _data = new List<SelectableItemData>();
    [SerializeField] private bool _hasAnimation;
    [SerializeField, ConditionalField(nameof(_hasAnimation))] private Animator _animator;
    [SerializeField, ConditionalField(nameof(_hasAnimation))] private string _animationBool = "Selected";

    [Header("Sounds")]
    [SerializeField] private Sound _hoverSound;
    [SerializeField] private Sound _selectSound;
    [SerializeField] private Sound _deselectSound;

    [Header("Events")]
    public UnityEvent OnSelect;
    public UnityEvent OnHover;
    public UnityEvent OnEndHover;
    public UnityEvent OnDeselect;

    [Header("Debug")]
    [SerializeField] private bool _printSelections;
    [SerializeField, ReadOnly] private bool _disabled;

    public bool Selected { get; private set; }
    public bool Hovered { get; private set; }
    public bool Disabled { get; private set; }

    private float _lastHoverTime = 0;

    private void OnValidate()
    {
        foreach (var d in _data) d.OnValidate();
    }

    private void Start()
    {
        if (_hoverSound) _hoverSound = Instantiate(_hoverSound);
        if (_selectSound) _selectSound = Instantiate(_selectSound);
        if (_deselectSound) _deselectSound = Instantiate(_deselectSound);

        //Disabled = false;
        if (_deselectOnStart) Deselect();
    }

    private void Update()
    {
        _disabled = Disabled;
    }

    [ButtonMethod]
    private void printTest()
    {
        print("buttonStatus: seleted: " + Selected);
    }

    [ButtonMethod]
    private void SetNormal()
    {
        foreach (var d in _data) d.Update(false, false, false);
#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }

    public void ToggleSelected()
    {
        if (Disabled) return;
        Selected = !Selected;
        UpdateVisuals();
    }

    public void Select()
    {
        if (_printSelections) print(gameObject.name + " selected");
        OnSelect.Invoke();
        if (_selectSound) _selectSound.Play(); 
        SetState(true);
    }
    public void Deselect()
    {
        if (_printSelections) print(gameObject.name + " deselected");
        OnDeselect.Invoke();
        if (_deselectSound) _deselectSound.Play();
        SetState(false);
    }

    public void SetState(bool selected)
    {
        if (Disabled) return;
        Selected = selected;
        UpdateVisuals();
    }

    public void SetEnabled(bool enabled)
    {
        //print(gameObject.name + " setting enab    xled: " + enabled);
        Disabled = !enabled;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (var d in _data) d.Update(Selected, Hovered, Disabled);
        if (_animator) _animator.SetBool(_animationBool, Selected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Disabled) return;
        if (Selected && (!_toggleOnClick || !_deselectOnClick)) return;
        OnHover.Invoke();

        if (_hasHoverCooldown) {
            var timeSinceLastHover = Time.time - _lastHoverTime;
            if (timeSinceLastHover < _hoverCooldown) return;
            _lastHoverTime = Time.time;
        }

        Hovered = true;
        if (_hoverSound) _hoverSound.Play();
        if (_selectOnHover) Select();
        else UpdateVisuals();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Disabled) return;
        if (Hovered) OnEndHover.Invoke();

        Hovered = false;
        if (_selectOnHover && _deselectOnExit) Deselect();
        else UpdateVisuals();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0)) return;

        if (Disabled) return;
        if (Selected && _toggleOnClick && _deselectOnClick) {
            Deselect();
            return;
        }

        if (_selectOnClick) {
            Select();
            if (!_toggleOnClick) Deselect();
        }
    }
}