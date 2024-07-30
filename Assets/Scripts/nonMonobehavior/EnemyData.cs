using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemyType {Goblin_Swordsman, Goblin_Mage}

[CreateAssetMenu(fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyType EnemyType;
    public GameObject Prefab;
    public int MaxHealth;
    public int MaxBlock;
    public int AttackDamage;
    public int BlockAmount;
    
}
