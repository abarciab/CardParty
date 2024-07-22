using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class PlayableCardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _img;
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private bool _canBeClicked;
    [SerializeField, ConditionalField(nameof(_canBeClicked))] private SelectableItem _seletable;

    private void OnEnable()
    {
        if (_seletable) _seletable.enabled = _canBeClicked;
    }

    public void Initialize(CardData card, string equipmentName = "") => Initialize(new CardInstance(card), equipmentName);

    public void Initialize(CardInstance card, string equipmentName = "")
    {
        _nameText.text = card.Name;
        _img.sprite = card.Sprite;
        _movesText.text = card.GetMoveData();
        _descriptionText.text = card.Description;
        _itemText.text = equipmentName;
        gameObject.SetActive(true);
    }

    public void AddOnClick(UnityAction callBack)
    {
        _seletable.OnSelect.AddListener(callBack);
    }
}
