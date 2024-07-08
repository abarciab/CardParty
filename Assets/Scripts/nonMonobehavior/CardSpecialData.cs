using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialType {NONE, HEAL, STUN, BURN, DRAW_CARDS}

[System.Serializable]
public class CardSpecialData
{
    [SerializeField] private SpecialType _type;
    [SerializeField, ConditionalField(nameof(_type), inverse:true, SpecialType.NONE)] private int _amount;

    public List<string> GetMoveData()
    {
        List<string> output = new List<string>();
        if (_type == SpecialType.HEAL) output.Add("Heal target " + Utilities.Parenthize(_amount));
        if (_type == SpecialType.HEAL) output.Add("Stun target for " + Utilities.Parenthize(_amount) + " turns");
        if (_type == SpecialType.HEAL) output.Add("Burn " + Utilities.Parenthize(_amount));
        if (_type == SpecialType.HEAL) output.Add("Draw " + Utilities.Parenthize(_amount) + " cards");
        return output;
    }
}
