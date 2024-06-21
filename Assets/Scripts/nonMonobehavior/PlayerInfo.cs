using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInfo
{
    public static PlayerStats Stats = new PlayerStats();
    public static PlayerInventory Inventory = new PlayerInventory();

    public static void InitializeEmpty()
    {
        Stats = new PlayerStats();
    }
}
