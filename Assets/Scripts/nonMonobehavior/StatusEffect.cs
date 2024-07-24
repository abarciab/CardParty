using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//status effects trigger in the left -> right defined order
public enum StatusEffectType {NONE, STUN}
public enum StatusEffectTriggerTime {STARTOFTURN, ENDOFTURN}
public class StatusEffect
{
    public StatusEffectType Type = StatusEffectType.NONE;
    public StatusEffectTriggerTime TriggerTime = StatusEffectTriggerTime.ENDOFTURN;
    public int Amount = 0;
    public int Duration = 0;

    public StatusEffect(StatusEffectData data) {
        Type = data.Type;
        TriggerTime = data.TriggerTime;
        Amount = data.Amount;
        Duration = data.Duration;
    }

    public StatusEffect(StatusEffectType newType, StatusEffectTriggerTime newTriggerTime, int newAmount, int newDuration) {
        Type = newType;
        TriggerTime = newTriggerTime;
        Amount = newAmount;
        Duration = newDuration;
    }

    public static StatusEffect operator +(StatusEffect a, StatusEffect b) {
        if (a.Type != b.Type) throw new System.Exception("Status effect type does not match");
        if (a.TriggerTime != b.TriggerTime) throw new System.Exception("Status effect trigger time does not match");

        return new StatusEffect(a.Type, a.TriggerTime, a.Amount + b.Amount, a.Duration + b.Duration);
    }

    public static bool operator ==(StatusEffect a, StatusEffect b) {
        if (a.Type != b.Type || a.TriggerTime != b.TriggerTime) return false;

        return true;
    }

    public static bool operator !=(StatusEffect a, StatusEffect b) {
        if (a.Type != b.Type || a.TriggerTime != b.TriggerTime) return true;

        return false;
    }
}