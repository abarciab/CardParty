using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    public StatusEffectType Type = StatusEffectType.NONE;
    public StatusEffectTriggerTime TriggerTime = StatusEffectTriggerTime.ENDOFTURN;
    public int Amount = 0;
    public int Duration = 0;
    
}