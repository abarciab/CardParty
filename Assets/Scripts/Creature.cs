using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public class Creature : MonoBehaviour
{
    public Canvas canvas;
    public Slider healthSlider;
    [SerializeField] int health;
    [SerializeField] int maxHealth;
    public Slider blockSlider;
    [SerializeField] int block = 0;
    [SerializeField] int maxBlock;

    public GameObject selectedCreatureHighlight;

    public void Select() {
        CardGameManager.i.SelectCreature(this);
    }

    public void Deselect() {
        CardGameManager.i.DeselectCreature(this);
    }

    public virtual IEnumerator Attack(float damage, Creature target) {
        yield return StartCoroutine(Utilities.LerpToAndBack(gameObject, target.transform.position));
        target.TakeDamage(damage);
        yield return null;
    }

    public virtual IEnumerator Block(float damage, Creature target) {
        yield return StartCoroutine(Utilities.LerpToAndBack(gameObject, target.transform.position));
        target.AddBlock(damage);
        yield return null;
    }

    public virtual void TakeDamage(float damage) {
        block = block - (int)damage;
        if (block < 0) {
            health += block;
            block = 0;
        }
        healthSlider.value = ((float)health / (float)maxHealth);
        blockSlider.value = ((float)block / (float)maxBlock);

        if (health <= 0) {
            StartCoroutine(Die());
        }
    }

    public virtual IEnumerator Die() {
        yield return StartCoroutine(Utilities.LerpScale(gameObject, Vector3.zero));
        yield return new WaitForSeconds(0.5f);
        CardGameManager.i.RemoveCreature(this);
        Destroy(gameObject);
    }

    public virtual void AddBlock(float block) {
        block += (int)block;
        blockSlider.value = ((float)block / (float)maxBlock);
    }
}