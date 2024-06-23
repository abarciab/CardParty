using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public class Creature : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Canvas canvas;
    public Slider healthSlider;
    [SerializeField] int health;
    [SerializeField] int maxHealth;
    public Slider blockSlider;
    [SerializeField] int block = 0;
    [SerializeField] int maxBlock;

    public GameObject selectedCreatureHighlight;
 
    public virtual void OnPointerUp(PointerEventData eventData) {
        //right click to display info
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        //right click to display info
    }

    public void Select() {
        CardGameManager.i.SelectCreature(this);
    }

    public void Deselect() {
        CardGameManager.i.DeselectCreature(this);
    }

    public virtual IEnumerator Attack(float damage, Creature target) {
        StartCoroutine(Utilities.LerpToAndBack(gameObject, target.transform.position));
        target.TakeDamage(damage);
        yield return null;
    }

    public virtual IEnumerator Block(float damage, Creature target) {
        StartCoroutine(Utilities.LerpToAndBack(gameObject, target.transform.position));
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
            Die();
        }
    }

    public virtual void Die() {
        CardGameManager.i.RemoveCreature(this);
        Destroy(gameObject);
    }

    public virtual void AddBlock(float block) {
        block += (int)block;
        blockSlider.value = ((float)block / (float)maxBlock);
    }
}
