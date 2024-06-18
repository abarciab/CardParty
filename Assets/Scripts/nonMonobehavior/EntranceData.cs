using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntranceData
{
    [HideInInspector] public string Name;
    public Direction Dir;
    public Transform TpPoint;
    public GameObject Door;
}
