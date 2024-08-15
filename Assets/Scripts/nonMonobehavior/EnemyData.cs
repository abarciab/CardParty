using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

[Serializable]
public class EnemyActionData {
    public EnemyActionType ActionType = EnemyActionType.None;
    public List<float> Amount;
    [ConditionalField (nameof(ActionType), inverse:false, EnemyActionType.Status, EnemyActionType.BuffAllies)] public StatusEffectData StatusEffectData;
}

[CreateAssetMenu(fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public string Name;
    public EnemyType EnemyType;
    public GameObject Prefab;
    public int MaxHealth;
    public int MaxBlock;

    public List<EnemyActionData> ActionData;
}