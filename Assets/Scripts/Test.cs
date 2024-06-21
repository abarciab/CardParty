using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [ButtonMethod]
    private void IncrementMoney()
    {
        PlayerInfo.Stats.Money += 1;
        print("money: " + PlayerInfo.Stats.Money);
    }

    [ButtonMethod]
    private void PrintInventory()
    {
        print("Inventory: " + PlayerInfo.Inventory);
    }
}
