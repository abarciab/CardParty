using System.Collections;
using System.Collections.Generic;
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
}
