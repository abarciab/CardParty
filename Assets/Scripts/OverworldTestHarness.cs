using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class OverworldTestHarness : MonoBehaviour
{
    [SerializeField] private List<EquipmentData> _testItemSet = new List<EquipmentData>();
    [SerializeField] private List<AdventurerData> _testAdventurerList = new List<AdventurerData>();
    [SerializeField] private List<EquipmentData> _testEquipmentLoad = new List<EquipmentData>();

    [SerializeField] private string _inputString = "banana";
    [SerializeField] private string _searchTerm = "b";

    [SerializeField] private int _testDamage;

    private void Start()
    {
        if (PlayerInfo.Party.Adventurers.Count == 0) LoadTestData();
    }

    [ButtonMethod]
    private void LoadTestData()
    {
        PlayerInfo.InitializeEmpty();
        PlayerInfo.Inventory.LoadItemList(_testItemSet);
        PlayerInfo.Party.SetParty(_testAdventurerList);

        foreach (var equipment in _testEquipmentLoad) {
            PlayerInfo.Party.SetEquipment(_testAdventurerList[0], equipment, equipment.Slot);
        }
    }

    [ButtonMethod]
    private void DamageFirstAdventurer()
    {
        PlayerInfo.Party.Adventurers[0].Stats.CurrentHealth -= _testDamage;
    }

    [ButtonMethod]
    public void LoadNormalPlaythroughData() {
        var standardOptions = Resources.LoadAll("TileInteractableOptions", typeof(TileInteractableData)).Cast<TileInteractableData>().ToList();
        FindObjectOfType<TileGenerator>().SetTileInteractableOptions(standardOptions);

        Utilities.SetDirty(FindObjectOfType<TileGenerator>());
    }

    public void OnValidate() {
        if (_testAdventurerList.Count > 0) _testAdventurerList.RemoveAll(x => x == null);
        if (_testAdventurerList.Count > 3) _testAdventurerList.RemoveRange(2, _testAdventurerList.Count - 3);

    }
}
