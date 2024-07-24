using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggeredEffectData")]
public class TriggeredEffectData : ScriptableObject
{
    public TriggeredEffectTriggerType Type;
    public TriggeredEffectTriggerTime TriggerTime;
    public int NumTriggers = -1; //indefinite
    public int Amount = 0;
}
