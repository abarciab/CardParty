using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Enemy : Creature
{
    public virtual IEnumerator Action(List<Adventurer> adventurers, List<Enemy> enemies) {
        //what the enemy does on their turn in combat
        yield return StartCoroutine(Attack(1, adventurers[Random.Range(0, adventurers.Count)]));
    }
}
