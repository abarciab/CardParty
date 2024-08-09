using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopGridItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private SelectableItem _button;
    [HideInInspector] public EquipmentData EquipmentData;
    private ShopController _controller;

    public void Initialize(EquipmentData data, ShopController controller)
    {
        _controller = controller;
        EquipmentData = data;

        _descriptionText.text = data.Description;
        _nameText.text = data.Name;
        _itemImg.sprite = data.Sprite;
        _priceText.text = data.Cost.ToString();
        RefreshButton();
    }

    public void RefreshButton() { }
    //public void RefreshButton() => _button.enabled = PlayerInfo.Stats.Money >= Equipment.Cost;

    public void ClickToBuy()
    {
        _controller.StopShowingCard(EquipmentData.Cards[0]);
        _controller.BuyItem(this);
    }

    private void Update()
    {
        if (!_button.Hovered) _controller.StopShowingCard(EquipmentData.Cards[0]);
    }

    public void ShowCard()
    {
        _controller.ShowCard(EquipmentData.Cards[0]);
    }
}
