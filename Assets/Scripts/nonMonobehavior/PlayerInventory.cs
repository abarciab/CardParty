using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory
{
    public List<EquipmentData> EquipmentData = new List<EquipmentData>();

    public override string ToString()
    {
        return string.Join(", ", EquipmentData);
    }

    public void AddEquipment(EquipmentData equipment)
    {
        EquipmentData.Add(Object.Instantiate(equipment));
    }

    public void LoadItemList(List<EquipmentData> equipmentList)
    {
        EquipmentData = new List<EquipmentData>(equipmentList);
    }
    
    public List<EquipmentData> GetValidItems(EquipmentSlot slot)
    {
        var list = EquipmentData.Where(x => x.Slot == slot).ToList();
        return new List<EquipmentData>(list); 
    }
}
