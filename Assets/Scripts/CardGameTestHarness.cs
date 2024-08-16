using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameTestHarness : MonoBehaviour
{
    [SerializeField] private List<EquipmentData> _testItemSet = new List<EquipmentData>();
    [SerializeField] private List<AdventurerData> _testParty = new List<AdventurerData>();
    [DisplayInspector] public CombatData TestCombat;
    [SerializeField] private List<EquipmentData> _testEquipmentLoad = new List<EquipmentData>();

    [SerializeField] private int _testDamage;

    [SerializeField] private bool _startTestEncounterOnStart = true;

    private void Start()
    {
        if (!OverworldManager.i) {
            LoadTestData();
            if (_startTestEncounterOnStart) StartTestEncounter();
        }
    }

    private void StartTestEncounter()
    {
        print("Starting test encounter");
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
