using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopGridItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Button _button;
    [HideInInspector] public Equipment Equipment;
    private ShopController _controller;

    public void Initialize(Equipment data, ShopController controller)
    {
        _controller = controller;
        Equipment = data;

        _nameText.text = data.Name;
        _priceText.text = data.Cost.ToString();
        RefreshButton();
    }

    public void RefreshButton() => _button.enabled = PlayerInfo.Stats.Money >= Equipment.Cost;

    public void ClickToBuy()
    {
        _controller.BuyItem(this);
    }
}
