using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryEquipmentGridItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;

    public void Initialize(Equipment data)
    {
        _name.text = data.Name;
    }
}
