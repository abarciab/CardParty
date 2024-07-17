using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory
{
    public List<Equipment> Equipment = new List<Equipment>();

    public override string ToString()
    {
        return string.Join(", ", Equipment);
    }

    public void AddEquipment(Equipment equipment)
    {
        Equipment.Add(Object.Instantiate(equipment));
    }

    public void LoadItemList(List<Equipment> equipmentList)
    {
        Equipment = new List<Equipment>(equipmentList);
    }
    
    public List<Equipment> GetValidItems(EquipmentSlot slot)
    {
        var list = Equipment.Where(x => x.Slot == slot).ToList();
        return new List<Equipment>(list); 
    }
}
