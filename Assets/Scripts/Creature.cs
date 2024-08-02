using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Events;
using MyBox;

public abstract class Creature : MonoBehaviour
{
    [SerializeField] protected CreatureObjectUIController UI;
    [SerializeField, ConditionalField(nameof(_gameRunning)), ReadOnly] protected int _health;
    [SerializeField] protected int _maxHealth;
    [SerializeField, ConditionalField(nameof(_gameRunning)), ReadOnly] private int _block = 0;
    [SerializeField] protected int _maxBlock;
    [SerializeField] protected Transform _model;

    [Header("Animation")]
    [SerializeField] protected Animator Animator;
    [SerializeField] private string _animWiggleBoolString = "wiggle";


    [HideInInspector] public CombatSlot CombatSlot;
    protected bool _isStunned = false;
    private Dictionary<StatusEffectTriggerTime, List<StatusEffect>> _statusEffects = new Dictionary<StatusEffectTriggerTime, List<StatusEffect>>();

    //public GameObject SelectedCreatureHighlight;
    protected TabletopController Controller;

    [SerializeField, HideInInspector] private bool _gameRunning;
    [HideInInspector] public UnityEvent<float> OnHealthPercentChanged;
    [HideInInspector] public UnityEvent<float> OnBlockPercentChanged;
    private bool _isSelectable;

    public void SetWiggle(bool state) => Animator.SetBool(_animWiggleBoolString, state);

    private void OnValidate()
    {
        _health = _maxHealth;
        _block = 0;
    }

    private void Start()
    {
        _gameRunning = true;
    }

    public abstract string GetName();

    public void MakeSelectable()
    {
        _isSelectable = true;
    }

    public void MakeUnselectable()
    {
        _isSelectable = false;
    }

    public override string ToString()
    {
        return gameObject.name;
    }

    public void ClickOn()
    {
        if (!_isSelectable) return;

        Controller.AddToSelectedTargets(this);
    }

    public void Initialize(TabletopController controller)
    {
        Controller = controller;
        _health = _maxHealth;
    }

    public virtual void TakeDamage(float damage) {
        _block -= Mathf.RoundToInt(damage);

        OnBlockPercentChanged.Invoke(_block / (float)_maxBlock);

        if (_block < 0) {
            _health += _block;
            _block = 0;
        }

        OnHealthPercentChanged.Invoke(_health / (float) _maxHealth);

        if (_health <= 0) Die();
    }

    public virtual void AddBlock(float blockDelta) {
        _block += Mathf.RoundToInt(blockDelta);
        OnBlockPercentChanged.Invoke(_block / (float)_maxBlock);
    }
    public virtual void RestoreHealth(int health) {
        _health += health;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
    }

    public virtual async void Die() {
        await Utilities.LerpScale(gameObject, Vector3.zero, 0.45f);
        await Task.Delay(500);
        Controller.RemoveCreature(this);
        if (gameObject != null) Destroy(gameObject);
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