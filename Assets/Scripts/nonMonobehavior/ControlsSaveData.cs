using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ControlData", menuName = "ControlsData")]
public class ControlsSaveData : ScriptableObject
{
    public List<MappedKeyData> Controls;
}
