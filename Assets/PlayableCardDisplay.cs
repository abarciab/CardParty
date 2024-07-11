using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayableCardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _img;
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _itemText;

    public void Initialize(CardData card, string equipmentName = "")
    {
        _nameText.text = card.Name;
        _img.sprite = card.Sprite;
        _movesText.text = card.GetMoveData();
        _descriptionText.text = card.Description;
        _itemText.text = equipmentName;
    }
}
