using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private int _moneyDelta;

    [ButtonMethod]
    private void IncrementMoney()
    {
        PlayerInfo.Stats.Money += _moneyDelta;
        print("money: " + PlayerInfo.Stats.Money);
    }

    [ButtonMethod]
    private void PrintInventory()
    {
        print("Inventory: " + PlayerInfo.Inventory);
    }
}
