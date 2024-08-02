using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Threading.Tasks;

public enum EnemyActionType {None, Attack, Block, Wait}
public class EnemyAction {
    public EnemyActionType Action;
    public CombatSlot TargetSlot;
    public EnemyAction(EnemyActionType newAction, CombatSlot newTargetSlot) {
        Action = newAction;
        TargetSlot = newTargetSlot;
    }
}

public class EnemyObject : Creature
{
    public EnemyType EnemyType;
    private EnemyAction _nextAction;
    [SerializeField] private GameObject _attackArrowPrefab;
    public AttackArrow AttackArrow;
    private float _attackDamage;
    private float _blockAmount;

    public void Initialize(EnemyData data) {
        EnemyType = data.EnemyType;
        _maxHealth = data.MaxHealth;
        _health = _maxHealth;
        _maxBlock = data.MaxBlock;
        _attackDamage = data.AttackDamage;
        _blockAmount = data.BlockAmount;

        UI.Initialize(this);
    }

    public async Task Action(List<AdventurerObject> adventurers, List<EnemyObject> enemies) {
        AdventurerObject target = (AdventurerObject)_nextAction.TargetSlot.Creature;
        if (!(_isStunned || _nextAction.Action == EnemyActionType.None || target == null)) {

            if (_nextAction.Action == EnemyActionType.Attack) {
                if (AttackArrow.BlockSlot.Creature) target = (AdventurerObject)AttackArrow.BlockSlot.Creature;

                await Utilities.LerpToAndBack(gameObject, target.transform.position);
                target.TakeDamage(_attackDamage);
            } else if (_nextAction.Action == EnemyActionType.Block) {
                AddBlock((_blockAmount));
            } else if (_nextAction.Action == EnemyActionType.Wait) {
                //pass
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
                if (_nextAction == null || _nextAction.Action == EnemyActionType.Attack) return new EnemyAction(EnemyActionType.Wait, target);
                if (_nextAction.Action == EnemyActionType.Wait) return new EnemyAction(EnemyActionType.Attack, target);
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