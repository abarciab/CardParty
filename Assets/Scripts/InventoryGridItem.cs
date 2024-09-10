using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SelectableItem))]
public class InventoryGridItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _previewName;
    SelectableItem _selector;
    [SerializeField] private float _hoverJumpAmount = 62;
    
    private float _startY;
    private InventoryUI _inventoryUI;
    public EquipmentData Data { get; private set; }

    private void Start()
    {
        _selector = GetComponent<SelectableItem>();
        _startY = transform.GetChild(0).localPosition.y;
    }

    public void Initialize(EquipmentData data, InventoryUI controller)
    {
        _inventoryUI = controller;
        Data = data;

        _name.text = data.Name;
        _previewName.text = data.Name;
    }

    public void SetHover(bool state)
    {
        var pos = transform.GetChild(0).localPosition;
        pos.y = _startY + (state ? _hoverJumpAmount : 0);
        transform.GetChild(0).localPosition = pos;
    }

    public void ClickOn()
    {
        _inventoryUI.ShowItemDetails(Data);
    }

}
