using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType {NONE, STUN}
public enum StatusEffectTriggerTime {STARTOFTURN, ENDOFTURN}
public class StatusEffect
{
    public StatusEffectType StatusEffectType = StatusEffectType.NONE;
    public StatusEffectTriggerTime StatusEffectTriggerTime = StatusEffectTriggerTime.ENDOFTURN;
    public int Amount = 0;
    public int Duration = 0;

    public StatusEffect(StatusEffectData statusEffectData) {
        StatusEffectType = statusEffectData.StatusEffectType;
        StatusEffectTriggerTime = statusEffectData.StatusEffectTriggerTime;
        Amount = statusEffectData.Amount;
        Duration = statusEffectData.Duration;
    }

    public StatusEffect(StatusEffectType newStatusEffectType, StatusEffectTriggerTime newStatusEffectTriggerTime, int newAmount, int newDuration) {
        StatusEffectType = newStatusEffectType;
        StatusEffectTriggerTime = newStatusEffectTriggerTime;
        Amount = newAmount;
        Duration = newDuration;
    }

    public static StatusEffect operator +(StatusEffect a, StatusEffect b) {
        if (a.StatusEffectType != b.StatusEffectType) throw new System.Exception("Status effect type does not match");
        if (a.StatusEffectTriggerTime != b.StatusEffectTriggerTime) throw new System.Exception("Status effect trigger time does not match");

        return new StatusEffect(a.StatusEffectType, a.StatusEffectTriggerTime, a.Amount + b.Amount, a.Duration + b.Duration);
    }

    public static bool operator ==(StatusEffect a, StatusEffect b) {
        if (a.StatusEffectType != b.StatusEffectType || a.StatusEffectTriggerTime != b.StatusEffectTriggerTime) return false;

        return true;
    }

    public static bool operator !=(StatusEffect a, StatusEffect b) {
        if (a.StatusEffectType != b.StatusEffectType || a.StatusEffectTriggerTime != b.StatusEffectTriggerTime) return true;

        return false;
    }
}