using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private List<Equipment> _testItemSet = new List<Equipment>();

    [SerializeField] private string _inputString = "banana";
    [SerializeField] private string _searchTerm = "b";

    private void Start()
    {
        LoadTestSet();
    }

    [ButtonMethod]
    private void LoadTestSet()
    {
        PlayerInfo.Inventory.LoadItemList(_testItemSet);
    }

    [ButtonMethod]
    private void TestContains()
    {
        print(_inputString + " contains " + _searchTerm + ": " + _inputString.Contains(_searchTerm));
    }
}
