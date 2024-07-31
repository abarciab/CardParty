using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Combat", menuName = "Combat")]
public class Combat : ScriptableObject
{
    public float difficulty;
    public float frequency;
    public EnemyData[] enemies;
}