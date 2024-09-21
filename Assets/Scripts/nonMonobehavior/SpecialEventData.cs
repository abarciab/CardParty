using JetBrains.Annotations;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class SpecialEventOutcome
{
    public EventOutcomeType Type;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.MONEY)] public int MoneyDelta;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.EQUIPMENT)] private EquipmentData _equipment;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.ADVENTURER_HIRE)] private AdventurerData _newHire;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.ADVENTURER_DAMAGE, EventOutcomeType.ADVENTURER_DAMAGE_ALL)] private int _damageAmount;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.MAP_REVEAL)] private int _numMapTiles;

    public void Trigger()
    {
        if (Type == EventOutcomeType.MONEY) PlayerInfo.Stats.Money += MoneyDelta;
        if (Type == EventOutcomeType.EQUIPMENT) PlayerInfo.Inventory.AddEquipment(_equipment);
        if (Type == EventOutcomeType.LOOT) PlayerInfo.AddLoot(Loot.GetLoot(OverworldManager.i.GetCurrentPlayerTile().GetDifficulty()));
        if (Type == EventOutcomeType.ADVENTURER_KILL) PlayerInfo.Party.KillRandomAdventurer();
        if (Type == EventOutcomeType.ADVENTURER_HIRE) PlayerInfo.Party.AddAdventurer(_newHire);
        if (Type == EventOutcomeType.ADVENTURER_DAMAGE) PlayerInfo.Party.DamageSingle(_damageAmount);
        if (Type == EventOutcomeType.ADVENTURER_DAMAGE_ALL) PlayerInfo.Party.DamageAll(_damageAmount);
        if (Type == EventOutcomeType.HEAL_FULL_PARTY) PlayerInfo.Party.HealAllAdventurers();
        if (Type == EventOutcomeType.MAP_REVEAL) OverworldUIManager.i.RevealRandomMapTiles(_numMapTiles);
        if (Type == EventOutcomeType.LOOT) PlayerInfo.Inventory.AddLoot();
        if (Type == EventOutcomeType.ADVENTURER_SWAP) PlayerInfo.Party.SwapAdventurer();
        if (Type == EventOutcomeType.EQUIPMENT_SWAP) PlayerInfo.Inventory.SwapEquipment();
        if (Type == EventOutcomeType.EQUIPMENT_DESTROY) PlayerInfo.Inventory.DestroyEquipment();
        if (Type == EventOutcomeType.HEAL_PARTY) PlayerInfo.Party.HealAllAdventurers(0.2f);
    }
}

[System.Serializable]
public class SpecialEventChoiceData
{
    public string Text;
    [Range(0, 1)] public float SuccessChance;
    [SerializeField] private string _successText;

    [Header("Success")]
    public SpecialEventOutcomeData SuccessOutcomeData;

    [Header("Failure")]
    [ConditionalField(true, nameof(CanFail))] public SpecialEventOutcomeData FailureOutcomeData;
    private bool CanFail() => SuccessChance < 1;

    public string GetPercent()
    {
        return Mathf.FloorToInt(SuccessChance * 100) + "% " + _successText;
    }
}

[System.Serializable]
[CreateAssetMenu(fileName ="SpecialEventData")]
public class SpecialEventData : ScriptableObject
{
    public string Title;
    [TextArea(3, 10)]public string Prompt;
    public Sprite Sprite;
    public int ChoicesToDisplay = 2;
    public List<SpecialEventChoiceData> Choices = new List<SpecialEventChoiceData>();

    public void OnValidate() {
        if (Title == "") Title = System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID()));
    }

    public bool IsValidEvent() {
        foreach (SpecialEventChoiceData choice in Choices) {
            foreach (SpecialEventOutcomeData outcomeData in new List<SpecialEventOutcomeData>{choice.SuccessOutcomeData, choice.FailureOutcomeData}) {
                foreach (SpecialEventOutcome outcome in outcomeData.Outcomes) {
                    if (outcome.Type == EventOutcomeType.MONEY && PlayerInfo.Stats.Money < outcome.MoneyDelta + Constants.MONEY_VARIANCE) return false;
                    if (outcome.Type == EventOutcomeType.ADVENTURER_KILL && PlayerInfo.Party.Adventurers.Count == 1) return false;
                    if (outcome.Type == EventOutcomeType.ADVENTURER_DAMAGE && PlayerInfo.Party.GetPartyHealthPercent() < 0.2f) return false;
                    if (outcome.Type == EventOutcomeType.ADVENTURER_DAMAGE_ALL && PlayerInfo.Party.GetPartyHealthPercent() < 0.2f) return false;
                    if (outcome.Type == EventOutcomeType.HEAL_FULL_PARTY && PlayerInfo.Party.GetPartyHealthPercent() > 0.8f) return false;
                    if (outcome.Type == EventOutcomeType.ADVENTURER_SWAP && PlayerInfo.Party.Adventurers.Count == 1) return false;
                    if (outcome.Type == EventOutcomeType.EQUIPMENT_SWAP && PlayerInfo.Inventory.GetAllEquipments().Count == 0) return false;
                    if (outcome.Type == EventOutcomeType.EQUIPMENT_DESTROY && PlayerInfo.Inventory.GetAllEquipments().Count == 0) return false;
                    if (outcome.Type == EventOutcomeType.HEAL_PARTY && PlayerInfo.Party.GetPartyHealthPercent() > 0.8f) return false;
                }
            }
        }
        return true;
    }
}

[System.Serializable]
public class SpecialEventOutcomeData {
    [TextArea(3, 10)] public string Text;
    public List<SpecialEventOutcome> Outcomes = new List<SpecialEventOutcome>();
}
