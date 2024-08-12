using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Threading.Tasks;

public enum EnemyType {Goblin_Swordsman, Goblin_Mage, Snake, Wolf, Goblin_Brute}

public enum EnemyActionType {None, Attack, Block, Wait, Status, BuffAllies, Stun}
public class EnemyAction {
    public List<EnemyActionType> Actions;
    public CombatSlot TargetSlot;
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
    public AttackArrow AttackArrow;
    private float _attackDamage;
    private float _blockAmount;
    private Dictionary<EnemyActionType, EnemyActionData> _actionData = new Dictionary<EnemyActionType, EnemyActionData>();

    public void Initialize(EnemyData data) {
        EnemyType = data.EnemyType;
        _maxHealth = data.MaxHealth;
        _health = _maxHealth;
        _maxBlock = data.MaxBlock;
        
        foreach(EnemyActionData actionData in data.ActionData) {
            _actionData.Add(actionData.ActionType, actionData);
        }

        UI.Initialize(this);
    }

    public async Task Action(List<AdventurerObject> adventurers, List<EnemyObject> enemies) {
        AdventurerObject target = (AdventurerObject)_nextAction.TargetSlot.Creature;
        if (!(_isStunned || _nextAction.Actions[0] == EnemyActionType.None || target == null)) {

            foreach(EnemyActionType type in _nextAction.Actions) {
                if (type == EnemyActionType.Attack) {
                    if (AttackArrow.BlockSlot.Creature) target = (AdventurerObject)AttackArrow.BlockSlot.Creature;

                    await Utilities.LerpToAndBack(gameObject, target.transform.position);
                    target.TakeDamage(_actionData[EnemyActionType.Attack].Amount[0]);
                } else if (type == EnemyActionType.Block) {
                    AddBlock(_actionData[EnemyActionType.Block].Amount[0]);
                } else if (type == EnemyActionType.Wait) {
                    //pass
                } else if (type == EnemyActionType.Status) {
                    target.AddStatusEffect(_actionData[EnemyActionType.Status].StatusEffectData);
                } else if (type == EnemyActionType.BuffAllies) {
                    foreach(EnemyObject enemy in CardGameManager.i.GetEnemies()) {
                        enemy.AddStatusEffect(_actionData[EnemyActionType.Status].StatusEffectData);
                    }
                }
            }
        }

        Controller.RemoveAttackArrow(AttackArrow);
    }

    public override string GetName() {
        return EnemyType.ToString();
    }

    public void ShowIntent() {
        _nextAction = GetAction();
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
        RotateToFaceTarget();
        DrawArrow();
    }

    private void RotateToFaceTarget()
    {
        if (!_nextAction.TargetSlot) return;

        var euler = _model.localEulerAngles;
        _model.LookAt(_nextAction.TargetSlot.transform.position);
        euler.y = _model.localEulerAngles.y;
        _model.localEulerAngles = euler;
    }

    private void DrawArrow() {
        if (!_nextAction.TargetSlot) return;

        var arrowStart = transform.position;
        var arrowEnd = _nextAction.TargetSlot.transform.position;

        if (!AttackArrow) CreateAttackArrow(arrowStart, arrowEnd);
        AttackArrow.Initialize(arrowStart, arrowEnd);
    }

    private void CreateAttackArrow(Vector3 start, Vector3 end)
    {
        AttackArrow = Instantiate(_attackArrowPrefab, transform.parent).GetComponent<AttackArrow>();

        var blockPos = Vector3.Lerp(start, end, 0.5f);
        CombatSlot newCombatSlot = Controller.SpawnBlockSlot(blockPos, this);

        newCombatSlot.AttackArrow = AttackArrow;
        AttackArrow.BlockSlot = newCombatSlot;
        AttackArrow.Owner = this;
    }

    public void SetTargetSlot(CombatSlot newTarget)
    {
        _nextAction.TargetSlot = newTarget;
        UpdateVisuals();
    }

    public override void Die()
    {
        Controller.RemoveAttackArrow(AttackArrow);
        base.Die();
    }
}