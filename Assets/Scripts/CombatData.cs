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
    public float difficulty;
    public float frequency;
    public EnemyData[] enemies;

    public void OnValidate() {
        int temp = 0;
        foreach (EnemyData enemy in enemies) {
            if (enemy != null) temp += EnemyCombatRatings[enemy.EnemyType];
        }
        difficulty = temp;
    }
}