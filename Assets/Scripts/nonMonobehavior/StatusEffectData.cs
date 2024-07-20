using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//status effects trigger in the left -> right defined order
[CreateAssetMenu(fileName = "StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    public StatusEffectType StatusEffectType = StatusEffectType.NONE;
    public StatusEffectTriggerTime StatusEffectTriggerTime = StatusEffectTriggerTime.ENDOFTURN;
    public int Amount = 0;
    public int Duration = 0;
    
}