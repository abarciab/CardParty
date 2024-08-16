using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class CombatListWrapper { public List<CombatData> items = new List<CombatData>(); }
[System.Serializable] public class EventListWrapper { public List<SpecialEventData> items = new List<SpecialEventData>(); }

[System.Serializable]
public class TileInteractableData
{
    public string Name;
    [Range(0, 1)] public float Difficulty;
    public TileInteractableType Type;
    public TileInteractableOutcome Outcome;
    [ConditionalField(nameof(Outcome), false, TileInteractableOutcome.SHOP)] public ShopData ShopData;
    [ConditionalField(nameof(Outcome), false, TileInteractableOutcome.EVENT)] public EventListWrapper EventOptions = new EventListWrapper();
    [ConditionalField(nameof(Outcome), false, TileInteractableOutcome.FIGHT)] public CombatListWrapper CombatOptions = new CombatListWrapper();
    [SerializeField, Min(1)]private int _frequency;
    [HideInInspector] public int Frequency => _frequency;
    [ReadOnly, Range(0, 1)] public float ActualChance = 0;

    public TileInteractableData() {}

    public TileInteractableData(TileInteractableData other)
    {
        if (other == null) return;

        Name = other.Name;
        Type = other.Type;
        Outcome = other.Outcome;
        if (other.ShopData) ShopData = Object.Instantiate(other.ShopData);
        foreach (var e in other.EventOptions.items) EventOptions.items.Add(Object.Instantiate(e));
        foreach (var c in other.CombatOptions.items) CombatOptions.items.Add(c);
    }
}
