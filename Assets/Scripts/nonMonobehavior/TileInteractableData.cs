using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileInteractableData
{
    public string Name;
    public TileInteractableType Type;
    public TileInteractableOutcome Outcome;
    [ConditionalField(nameof(Outcome), false, TileInteractableOutcome.SHOP)] public ShopData ShopData;

    public TileInteractableData() {}

    public TileInteractableData(TileInteractableData other)
    {
        Name = other.Name;
        Type = other.Type;
        Outcome = other.Outcome;
        if (other.ShopData) ShopData = Object.Instantiate(other.ShopData);
    }
}
