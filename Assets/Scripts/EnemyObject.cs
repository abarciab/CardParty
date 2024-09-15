using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;

public enum EnemyType {Goblin_Swordsman, Goblin_Mage, Snake, Wolf, Goblin_Brute}

public enum EnemyActionType {None, Attack, Block, Wait, Status, BuffAllies}

public class EnemyAction {
    public List<EnemyActionType> Actions;
    public CombatSlot TargetSlot;
    public bool needsArrow => TargetSlot && (Actions[0] == EnemyActionType.Attack || Actions[0] == EnemyActionType.Status);

    public EnemyAction(List<EnemyActionType> newActions, CombatSlot newTargetSlot) {
        Actions = newActions;
        TargetSlot = newTargetSlot;
    }
    public EnemyAction(EnemyActionType newAction, CombatSlot newTargetSlot) {
        Actions = new List<EnemyActionType>(){newAction};
        TargetSlot = newTargetSlot;
    }
}

public class EnemyObject : CreatureObject
{
    public EnemyType EnemyType;
    private EnemyAction _nextAction;
    [SerializeField] private GameObject _attackArrowPrefab;
    public AttackArrow attackArrow;
    private float _attackDamage;
    private float _blockAmount;
    private Dictionary<EnemyActionType, EnemyActionData> _actionData = new Dictionary<EnemyActionType, EnemyActionData>();
    private EnemyData _data;

    [SerializeField] private Sound _attackSound;

    public CombatSlot SpawnBlockSlot(Vector3 pos) => Controller.SpawnBlockSlot(pos, this);
    public override string GetName() => _data.Name;

    private CardGameManager _gMan => CardGameManager.i;

    protected override void Start()
    {
        base.Start();
        _attackSound = Instantiate(_attackSound);
    }

    public void Initialize(EnemyData data) {
        _maxHealth = data.MaxHealth;
        _health = _maxHealth;
        _maxBlock = data.MaxBlock;
        _data = data;

        foreach(EnemyActionData actionData in data.ActionData) {
            _actionData.Add(actionData.ActionType, actionData);
        }

        UI.Initialize(this);
        UpdateUI(false);
    }

    public async Task TakeAction() {
        AdventurerObject target = _nextAction.needsArrow ? attackArrow.TargetAdventurer : null;
        if (_isStunned || _nextAction.Actions[0] == EnemyActionType.None) return;

        foreach (EnemyActionType type in _nextAction.Actions) await EvaluateAction(type, target);
        Controller.RemoveAttackArrow(attackArrow);
    }

    private async Task EvaluateAction(EnemyActionType action, AdventurerObject target)
    {
        if (action == EnemyActionType.Block) AddBlock(_actionData[EnemyActionType.Block].Amount[0]);
        if (action == EnemyActionType.Status) target.AddStatusEffect(_actionData[EnemyActionType.Status].StatusEffectData);
        if (action == EnemyActionType.Attack) await AttackTarget(attackArrow.TargetAdventurer);
        if (action == EnemyActionType.BuffAllies) foreach (var e in _gMan.GetEnemies()) e.AddStatusEffect(_actionData[EnemyActionType.BuffAllies].StatusEffectData);
    }

    private async Task AttackTarget(AdventurerObject target)
    {
        if (target == null) return;
        _attackSound.Play();
        await Utilities.LerpToAndBack(gameObject, target.transform.position);
        target.TakeDamage((int)_actionData[EnemyActionType.Attack].Amount[0] + GetBonusDamage());
    }

    public void ShowIntent() {
        _nextAction = GetAction();
        //if (_nextAction.TargetSlot) _nextAction.TargetSlot.IsLocked = true;
        UpdateVisuals();
    }

    private EnemyAction GetAction() {
        if (EnemyType == EnemyType.Goblin_Swordsman) {
            var target = Controller.GetValidAttackTarget(CombatSlot);
            if (target != null) return new EnemyAction(EnemyActionType.Attack, target);

        } else if (EnemyType == EnemyType.Goblin_Mage) {
            var target = Controller.GetValidAttackTarget(CombatSlot);
            if (target != null) {
                if (_nextAction == null || _nextAction.Actions[0] == EnemyActionType.Attack) return new EnemyAction(EnemyActionType.Wait, target);
                if (_nextAction.Actions[0] == EnemyActionType.Wait) return new EnemyAction(EnemyActionType.Attack, target);
            }
        }
        else if (EnemyType == EnemyType.Snake) {
            var target = Controller.GetValidAttackTarget(CombatSlot);
            if (target != null) {
                if (_nextAction == null) return new EnemyAction(EnemyActionType.Status, target);
                if (_nextAction.Actions[0] == EnemyActionType.Block) return new EnemyAction(EnemyActionType.Status, target);
                if (_nextAction.Actions[0] == EnemyActionType.Status) return new EnemyAction(EnemyActionType.Block, target);
            }
        }
        else if (EnemyType == EnemyType.Wolf) {
            var target = Controller.GetValidAttackTarget(CombatSlot);
            if (target == null) {
                return new EnemyAction(EnemyActionType.BuffAllies, target);
            } else {
                float rand = UnityEngine.Random.Range(0, 3);
                if (rand < 1) {
                    return new EnemyAction(EnemyActionType.BuffAllies, target);
                } else if (rand < 2) {
                    return new EnemyAction(EnemyActionType.Attack, target);
                } else {
                    return new EnemyAction(EnemyActionType.Block, target);
                }
            }
        }
        else if (EnemyType == EnemyType.Goblin_Brute) {
            var target = Controller.GetValidAttackTarget(CombatSlot);
            if (target == null) {
                return new EnemyAction(EnemyActionType.BuffAllies, target);
            } else {
                return new EnemyAction(new List<EnemyActionType>() {
                    EnemyActionType.Attack,
                    EnemyActionType.Status
                }, target);
            }
        }

        return new EnemyAction(EnemyActionType.None, null);
    }

    public CombatSlot GetTarget() {
        return _nextAction.TargetSlot;
    }

    public void UpdateVisuals()
    {
        if (_nextAction.TargetSlot) RotateToFaceTarget();
        if (_nextAction.needsArrow)DrawArrow();
    }

    private void RotateToFaceTarget()
    {
        var euler = _model.localEulerAngles;
        _model.LookAt(_nextAction.TargetSlot.transform.position);
        euler.y = _model.localEulerAngles.y;
        _model.localEulerAngles = euler;
    }

    private void DrawArrow() {
        if (!attackArrow) attackArrow = Instantiate(_attackArrowPrefab, transform.parent).GetComponent<AttackArrow>();
        attackArrow.Initialize(this, _nextAction.TargetSlot, (int)_actionData[EnemyActionType.Attack].Amount[0] + GetBonusDamage());
    }

    public void SetTargetSlot(CombatSlot newTarget)
    {
        _nextAction.TargetSlot = newTarget;
        UpdateVisuals();
    }

    public override void Die()
    {
        Controller.RemoveAttackArrow(attackArrow);
        base.Die();
    }

}