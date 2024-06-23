using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Combat", menuName = "Encounter/Combat")]
public class Combat : Encounter
{
    public float difficulty;
    public float frequency;
    public GameObject[] enemies;
}