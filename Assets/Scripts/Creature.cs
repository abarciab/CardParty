using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

public class Creature : MonoBehaviour
{
    public Canvas Canvas;
    public CombatSlot CombatSlot;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _blockSlider;
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _block = 0;
    [SerializeField] private int _maxBlock;

    public GameObject SelectedCreatureHighlight;

    public void Select() {
        CardGameManager.i.SelectCreature(this);
    }

    public void Deselect() {
        CardGameManager.i.DeselectCreature(this);
    }

    public virtual void TakeDamage(float damage) {
        _block = _block - (int)damage;
        if (_block < 0) {
            _health += _block;
            _block = 0;
        }
        _healthSlider.value = ((float)_health / (float)_maxHealth);
        _blockSlider.value = ((float)_block / (float)_maxBlock);

        if (_health <= 0) {
            Die();
        }
    }

    public virtual void AddBlock(float block) {
        _block += (int)block;
        _blockSlider.value = ((float)_block / (float)_maxBlock);
    }

    public void Die() { Die_Coroutine(); }

    protected virtual async Task Die_Coroutine() {
        await Utilities.LerpScale(gameObject, Vector3.zero);
        await Task.Delay(500);
        CardGameManager.i.RemoveCreature(this);
        Destroy(gameObject);
    }
}