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
    [SerializeField, ConditionalField(nameof(_type), inverse: true, SpecialType.NONE)] private bool _targetAll;

    public List<string> GetMoveData()
    {
        List<string> output = new List<string>();
        string targetString = _targetAll ? "all" : "target";
        if (_type == SpecialType.HEAL) output.Add("Heal " + targetString + " " + Utilities.Parenthize(_amount));
        if (_type == SpecialType.STUN) output.Add("Stun " + targetString + " for " + Utilities.Parenthize(_amount) + " turns");
        if (_type == SpecialType.BURN) output.Add("Burn " + Utilities.Parenthize(_amount));
        if (_type == SpecialType.DRAW_CARDS) output.Add("Draw " + Utilities.Parenthize(_amount) + " cards");
        return output;
    }
}
