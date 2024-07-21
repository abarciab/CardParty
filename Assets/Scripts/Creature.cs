using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;
using System.Linq;

public class Creature : MonoBehaviour
{
    public Canvas Canvas;
    [MyBox.ReadOnly] public CombatSlot CombatSlot;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _blockSlider;
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _block = 0;
    [SerializeField] private int _maxBlock;
    protected bool _isStunned = false;
    private Dictionary<StatusEffectTriggerTime, List<StatusEffect>> _statusEffects = new Dictionary<StatusEffectTriggerTime, List<StatusEffect>>();

    public GameObject SelectedCreatureHighlight;
    protected TabletopController Controller;

    public void Initialize(TabletopController controller)
    {
        Controller = controller;
    }

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
        _healthSlider.value = (_health / (float)_maxHealth);
        _blockSlider.value = (_block / (float)_maxBlock);

        if (_health <= 0) Die();
    }

    public virtual void AddBlock(float block) {
        _block += (int)block;
        _blockSlider.value = (_block / (float)_maxBlock);
    }

    public virtual async void Die() {
        await Utilities.LerpScale(gameObject, Vector3.zero);
        await Task.Delay(500);
        Controller.RemoveCreature(this);
        Destroy(gameObject);
    }

    public void AddStatusEffect(StatusEffectData statusEffectData) {
        StatusEffect newStatus = new StatusEffect(statusEffectData);
        StatusEffectTriggerTime newTime = newStatus.TriggerTime;

        if (!_statusEffects.Keys.Contains(newTime)) {
            _statusEffects.Add(newTime, new List<StatusEffect>());
        } else {
            for (int i = 0; i < _statusEffects[newTime].Count; i++) {
                if (newStatus.Type == _statusEffects[newTime][i].Type) {
                    _statusEffects[newTime][i] = _statusEffects[newTime][i] + newStatus;
                    return;
                }
            }
        }
        _statusEffects[newTime].Add(newStatus);
        _statusEffects[newTime].OrderBy(x => (int)x.Type);
    }

    private void RemoveStatusEffect(StatusEffect status) {
        _statusEffects[status.TriggerTime].Remove(status);
    }

    public void TriggerStatusEffects(StatusEffectTriggerTime time) {
        if (!_statusEffects.Keys.Contains(time)) return;

        List<StatusEffect> toRemove = new List<StatusEffect>();

        foreach (StatusEffect status in _statusEffects[time]) {
            if (status.Type == StatusEffectType.STUN) {
                if (status.Duration == 0) {
                    _isStunned = false;
                    toRemove.Add(status);
                } else {
                    _isStunned = true;
                    status.Duration--;
                }
            }
        }

        foreach (StatusEffect status in toRemove) {
            RemoveStatusEffect(status);
        }
    }
}