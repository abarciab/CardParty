using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor.ShaderGraph.Internal;
using System.Xml.Serialization;

public enum SelectableItemDataType { GRAPHIC, GAMEOBJECT}

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

    private bool _isGraphic => _type == SelectableItemDataType.GRAPHIC;
    private bool _isGameObject => _type == SelectableItemDataType.GAMEOBJECT;

    public void OnValidate()
    {
        if ((_isGraphic && !_target) || (_isGameObject && !_obj)) Initialize();
        else Name = _isGraphic ? _target.gameObject.name : _obj.name;
    }

    private void Initialize()
    {
        _selectedColor = Color.white;
        _hoveredColor = Color.white;
        _normalColor = Color.white;
        _disabledColor = Color.white;
        _selectedState = true;
        _hoveredState = false;
        _normalState = false;
        _disabledState = false;
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
        if (_type == SelectableItemDataType.GRAPHIC) _target.color = _selectedColor;
        else _obj.SetActive(_selectedState);
    }

    private void Hover()
    {
        if (_type == SelectableItemDataType.GRAPHIC) _target.color = _hoveredColor;
        else _obj.SetActive(_hoveredState);
    }

    private void Deselect()
    {
        if (_type == SelectableItemDataType.GRAPHIC) _target.color = _normalColor;
        else _obj.SetActive(_normalState);
    }

    private void Disable()
    {
        if (_type == SelectableItemDataType.GRAPHIC) _target.color = _disabledColor;
        else _obj.SetActive(_disabledState);
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
    public UnityEvent OnDeselect;

    [Header("Debug")]
    [SerializeField] private bool _printSelections;

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

        Disabled = false;
        if (_deselectOnStart) Deselect();
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
        Hovered = false;
        if (_selectOnHover && _deselectOnExit) Deselect();
        else UpdateVisuals();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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