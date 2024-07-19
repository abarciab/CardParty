using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameTestHarness : MonoBehaviour
{
    [SerializeField] private List<Equipment> _testItemSet = new List<Equipment>();
    [SerializeField] private List<AdventurerData> _testParty = new List<AdventurerData>();
    public Combat TestCombat;
    [SerializeField] private List<Equipment> _testEquipmentLoad = new List<Equipment>();

    [SerializeField] private int _testDamage;

    [SerializeField] private bool _startTestEncounterOnStart = true;

    private void Start()
    {
        if (!OverworldManager.i) LoadTestData();
        if (_startTestEncounterOnStart) StartTestEncounter();
    }

    private void StartTestEncounter()
    {
        CardGameManager.i.StartCombat(TestCombat);
    }

    [ButtonMethod]
    private void LoadTestData()
    {
        PlayerInfo.InitializeEmpty();
        PlayerInfo.Inventory.LoadItemList(_testItemSet);
        PlayerInfo.Party.SetParty(_testParty);

        foreach (var equipment in _testEquipmentLoad) {
            PlayerInfo.Party.SetEquipment(_testParty[0], equipment, equipment.Slot);
        }
    }

    [ButtonMethod]
    private void DamageFirstAdventurer()
    {
        PlayerInfo.Party.Adventurers[0].Stats.CurrentHealth -= _testDamage;
    }

    [ButtonMethod] private void PrintParty() => print(PlayerInfo.Party.ToString());
}
