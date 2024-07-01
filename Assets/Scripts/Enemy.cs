using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Enemy : Creature
{

    public IEnumerator Action(List<Adventurer> adventurers, List<Enemy> enemies) {
        //what the enemy does on their turn in combat
        Adventurer target = adventurers[Random.Range(0, adventurers.Count)];

        yield return StartCoroutine(Utilities.LerpToAndBack(gameObject, target.transform.position));
        target.TakeDamage(50);
    }
}
