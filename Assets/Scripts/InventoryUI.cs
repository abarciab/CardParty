using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _equipmentGridItemPrefab;
    [SerializeField] private Transform _gridParent;

    private List<GameObject> _gridItems = new List<GameObject>();

    public void Show()
    {
        foreach (var item in _gridItems) Destroy(item.gameObject);
        _gridItems.Clear();

        gameObject.SetActive(true);
        var equipment = PlayerInfo.Inventory.Equipment;
        foreach (var e in equipment) DisplayEquipment(e);
    }

    private void DisplayEquipment(Equipment data)
    {
        var newGridItem = Instantiate(_equipmentGridItemPrefab, _gridParent);
        newGridItem.GetComponent<InventoryEquipmentGridItem>().Initialize(data);
        _gridItems.Add(newGridItem);
    }
}
