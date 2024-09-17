using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory
{
    public List<EquipmentData> Equipments = new List<EquipmentData>();

    public override string ToString()
    {
        return string.Join(", ", Equipments);
    }

    public void AddLoot() {
        Loot res = Loot.GetLoot(0);

        PlayerInfo.Stats.Money += res.Gold;
        if (res.Equipment) AddEquipment(res.Equipment);
    }

    public void AddEquipment(EquipmentData equipment, int difficulty = -1)
    {
        if (!equipment) {
            if (difficulty == -1) {
                difficulty = Constants.MAX_DIFFICULTY;
            }

            equipment = EquipmentData.GetEquipment(difficulty);
        }
        Equipments.Add(Object.Instantiate(equipment));
    }

    public void DestroyEquipment(EquipmentData equipment = null) {        

        if (!equipment) {
            if (PlayerInfo.Party.GetEquippedEquipment()) {
                equipment = PlayerInfo.Party.GetEquippedEquipment();
            } else {
                foreach(EquipmentData e in Equipments) {
                    Debug.Log(e);
                }
                equipment = Equipments[Random.Range(0, Equipments.Count)];
            }
        }

        if (!equipment) {
            Debug.Log("failed");
            return;
        }

        if (PlayerInfo.Party.GetEquippedEquipment()) PlayerInfo.Party.SetEquipment(PlayerInfo.Party.GetOwner(equipment), null, equipment.Slot);

        Debug.Log("removing " + equipment.Name);
        Equipments.Remove(equipment);
    }

    public void SwapEquipment(EquipmentData oldEquipment = null, EquipmentData newEquipment = null, int difficulty = -1) {
        if (difficulty == -1) difficulty = Constants.MAX_DIFFICULTY;

        if (!newEquipment) newEquipment = EquipmentData.GetEquipment(difficulty);

        if (!oldEquipment) oldEquipment = PlayerInfo.Party.GetEquippedEquipment();

        AddEquipment(newEquipment);

        DestroyEquipment(oldEquipment);
    }

    public void LoadItemList(List<EquipmentData> equipmentList)
    {
        Equipments = new List<EquipmentData>(equipmentList);
    }
    
    public List<EquipmentData> GetValidItems(EquipmentSlot slot)
    {
        var list = Equipments.Where(x => x.Slot == slot).ToList();
        return new List<EquipmentData>(list); 
    }

    public List<EquipmentData> GetAllEquipments() {
        var temp = Equipments;
        temp.AddRange(PlayerInfo.Party.GetAllEquippedItems());
        return temp;
    }
}
