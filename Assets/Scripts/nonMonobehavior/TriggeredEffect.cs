using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//effects trigger in the left -> right defined order
public enum TriggeredEffectTriggerType { DRAW }
public enum TriggeredEffectTriggerTime { STARTOFPLAYERTURN, ENDOFPLAYERTURN, STARTOFENEMYTURN, ENDOFENEMYTURN }
public class TriggeredEffect
{
    public TriggeredEffectTriggerType Type;
    public TriggeredEffectTriggerTime TriggerTime;
    public int NumTriggers = -1; //indefinite
    public int Amount = 0;

    public TriggeredEffect(TriggeredEffectData data) {
        TriggerTime = data.TriggerTime;
        NumTriggers = data.NumTriggers;
        Amount = data.Amount;
    }

    public TriggeredEffect(TriggeredEffectTriggerType newTriggerType, TriggeredEffectTriggerTime newTriggerTime, int newNumTriggers, int newAmount) {
        Type = newTriggerType;
        TriggerTime = newTriggerTime;
        NumTriggers = newNumTriggers;
        Amount = newAmount;
    }

    public static TriggeredEffect operator +(TriggeredEffect a, TriggeredEffect b) {
        if (a.Type != b.Type) throw new System.Exception("Trigger effect type does not match");
        if (a.TriggerTime != b.TriggerTime) throw new System.Exception("Trigger effect trigger time does not match");
        if (a.Amount != b.Amount) throw new System.Exception("Trigger effect amount does not match");

        return new TriggeredEffect(a.Type, a.TriggerTime, a.NumTriggers + b.NumTriggers, a.Amount);
    }

    public static bool operator ==(TriggeredEffect a, TriggeredEffect b) {
        if (a.Type != b.Type || a.TriggerTime != b.TriggerTime || a.Amount != b.Amount) return false;

        return true;
    }

    public static bool operator !=(TriggeredEffect a, TriggeredEffect b) {
        if (a.Type != b.Type || a.TriggerTime != b.TriggerTime || a.Amount != b.Amount) return true;

        return false;
    }

    public void Trigger() {
        if (Type == TriggeredEffectTriggerType.DRAW) {
            CardGameUIManager.i.Draw(Amount);
        }
    }
}
