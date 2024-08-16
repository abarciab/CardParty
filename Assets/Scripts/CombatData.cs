using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]

[CreateAssetMenu(fileName = "Combat", menuName = "Combat")]
public class CombatData : ScriptableObject
{
    public Dictionary<EnemyType, int> EnemyCombatRatings = new Dictionary<EnemyType, int>(){
    {EnemyType.Goblin_Swordsman, 1},
    {EnemyType.Goblin_Mage, 2},
    {EnemyType.Snake, 3},
    {EnemyType.Wolf, 4},
    {EnemyType.Goblin_Brute, 10}
    };
    public float Difficulty;
    public float Frequency;
    public List<EnemyData> Enemies = new List<EnemyData>();

    public void OnValidate() {
        if (Enemies.Count == 0) return;

        int temp = 0;
        foreach (EnemyData enemy in Enemies) {
            if (enemy != null) temp += EnemyCombatRatings[enemy.EnemyType];
        }
        Difficulty = temp;
    }
}