using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private List<Equipment> _testItemSet = new List<Equipment>();
    [SerializeField] private List<AdventurerData> _testAdventurerList = new List<AdventurerData>();
    [SerializeField] private List<Equipment> _testEquipmentLoad = new List<Equipment>();

    [SerializeField] private string _inputString = "banana";
    [SerializeField] private string _searchTerm = "b";

    private void Start()
    {
        LoadTestData();
    }

    [ButtonMethod]
    private void LoadTestData()
    {
        PlayerInfo.Inventory.LoadItemList(_testItemSet);
        PlayerInfo.Party.SetParty(_testAdventurerList);

        foreach (var equipment in _testEquipmentLoad) {
            PlayerInfo.Party.SetEquipment(_testAdventurerList[0], equipment, equipment.Slot);
        }
    }
}
