using JetBrains.Annotations;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class SpecialEventOutcome
{
    public EventOutcomeType Type;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.MONEY)] private int _moneyDelta;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.EQUIPMENT)] private Equipment _equipment;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.ADVENTURER_HIRE)] private AdventurerData _newHire;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.ADVENTURER_DAMAGE)] private int _damageAmount;
    [SerializeField, ConditionalField(nameof(Type), false, false, EventOutcomeType.MAP_REVEAL)] private int _numMapTiles;

    public void Trigger()
    {
        if (Type == EventOutcomeType.MONEY) PlayerInfo.Stats.Money += _moneyDelta;
        if (Type == EventOutcomeType.EQUIPMENT) PlayerInfo.Inventory.AddEquipment(_equipment);
        if (Type == EventOutcomeType.FIGHT) OverworldManager.i.LoadCardGame();
        if (Type == EventOutcomeType.ADVENTURER_KILL) PlayerInfo.Party.KillRandomAdventurer();
        if (Type == EventOutcomeType.ADVENTURER_HIRE) PlayerInfo.Party.AddAdventurer(_newHire);
        if (Type == EventOutcomeType.ADVENTURER_DAMAGE) PlayerInfo.Party.DamageAll(_damageAmount);
        if (Type == EventOutcomeType.PARTY_FULL_HEAL) PlayerInfo.Party.HealAllAdventurers();
        if (Type == EventOutcomeType.MAP_REVEAL) OverworldUIManager.i.RevealRandomMapTiles(_numMapTiles);
    }
}

[System.Serializable]
public class SpecialEventChoiceData
{
    public string Text;
    [Range(0, 1)] public float SuccessChance;
    [SerializeField] private string _successText;

    [Header("Success")]
    [TextArea (3, 10) ] public string SuccessText;
    public List<SpecialEventOutcome> SucessOutcomes = new List<SpecialEventOutcome>();

    [Header("failure")]
    [TextArea(3, 10)] public string FailText;
    public List<SpecialEventOutcome> FailureOutcomes = new List<SpecialEventOutcome>();

    public string GetPercent()
    {
        return Mathf.FloorToInt(SuccessChance * 100) + "% " + _successText;
    }
}

[CreateAssetMenu(fileName ="new special event")]
public class SpecialEventData : ScriptableObject
{
    public string Title;
    [TextArea(3, 10)]public string Prompt;
    public Sprite Sprite;
    public int ChoicesToDisplay = 2;
    public List<SpecialEventChoiceData> Choices = new List<SpecialEventChoiceData>();
}
