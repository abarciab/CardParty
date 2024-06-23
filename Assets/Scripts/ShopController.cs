using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject _gridItemPrefab;
    [SerializeField] private Transform _gridParent;

    [SerializeField] private List<Equipment> _options = new List<Equipment>();
    [SerializeField] private int _numToShow = 3;
    private List<ShopGridItem> _gridItems = new List<ShopGridItem>();

    public void OpenShop()
    {
        ClearGridItems();
        _options = (List<Equipment>)_options.Shuffle();
        for (int i = 0; i < _numToShow; i++) {
            ShowItem(_options[i]);
        }
        gameObject.SetActive(true);
    }

    private void ClearGridItems()
    {
        foreach (var item in _gridItems) Destroy(item.gameObject);
        _gridItems.Clear();
    }

    private void ShowItem(Equipment item)
    {
        var newGridItem = Instantiate(_gridItemPrefab, _gridParent);
        var itemScript = newGridItem.GetComponent<ShopGridItem>();
        itemScript.Initialize(item, this);
        _gridItems.Add(itemScript);
    }

    public void BuyItem(ShopGridItem gridItem)
    {
        var toBuy = gridItem.Equipment;
        Destroy(gridItem.gameObject);

        PlayerInfo.Stats.Money -= toBuy.Cost;
        PlayerInfo.Inventory.AddEquipment(toBuy);

        RecalculateCosts();
    }

    private void RecalculateCosts()
    {
        foreach (var item in _gridItems) item.RefreshButton();
    }

    public void CloseShop()
    {
        OverworldUIManager.i.CloseShop();
    }
}
