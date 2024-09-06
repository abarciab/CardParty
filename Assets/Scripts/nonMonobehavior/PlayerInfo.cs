using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInfo
{
    public static PlayerStats Stats { get; private set; } = new PlayerStats();
    public static PlayerInventory Inventory { get; private set; } = new PlayerInventory();
    public static Party Party { get; private set; } = new Party();

    public static void InitializeEmpty()
    {
        Stats = new PlayerStats();
        Inventory = new PlayerInventory();
        Party = new Party();
    }

    public static void AddLoot(Loot loot) {
        if (loot.Equipment) Inventory.AddEquipment(loot.Equipment);
        Stats.Money += loot.Gold;
    }
}