using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySelectedCard : MonoBehaviour
{
    [Header("EquipmentCard")]
    [SerializeField] private TextMeshProUGUI _eCardNameText;
    [SerializeField] private Image _eCardImg;
    [SerializeField] private TextMeshProUGUI _eCardDescriptionText;
    [SerializeField] private TextMeshProUGUI _eCardHolderText;
    [SerializeField] private TextMeshProUGUI _eCardSlotText;
    [SerializeField] private TextMeshProUGUI _eCardValueText;

    [Header("PlayCard")]
    [SerializeField] private PlayableCardDisplay _pCardDisplay;

    public void Initialize(Equipment data)
    {
        _eCardNameText.text = data.Name;
        _eCardImg.sprite = data.Sprite;
        _eCardDescriptionText.text = data.Description;

        var owner = PlayerInfo.Party.GetOwner(data);
        if (owner != null) _eCardHolderText.text = "Currently " + data.WornByString + " by " + owner.Name;
        _eCardHolderText.gameObject.SetActive(owner != null);

        _eCardSlotText.text = data.Slot.ToString();
        _eCardValueText.text = data.Cost.ToString();

        bool hasCards = data.Cards.Count > 0;
        _pCardDisplay.gameObject.SetActive(hasCards);
        if (!hasCards) return;

        var card = data.Cards[0];
        _pCardDisplay.Initialize(card, data.Name);
    }
}
