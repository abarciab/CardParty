using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Threading.Tasks;
using TMPro.EditorUtilities;

public enum EnemyActionType {None, Attack}
public enum EnemyType {Goblin}
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
    [SerializeField] private float _attackDamage;

    public async Task Action(List<AdventurerObject> adventurers, List<EnemyObject> enemies) {
        switch(_nextAction.Action) {
            case EnemyActionType.Attack: {
                AdventurerObject target = (AdventurerObject)_nextAction.TargetSlot.Creature;
                    if (target == null) break;
                if (AttackArrow.BlockSlot.Creature) target = (AdventurerObject)AttackArrow.BlockSlot.Creature;

                await Utilities.LerpToAndBack(gameObject, target.transform.position);
                target.TakeDamage(_attackDamage);
            }
            break;
        }

        Controller.ClearBlockSlot(AttackArrow.BlockSlot);
        Destroy(AttackArrow.BlockSlot.gameObject);
        Destroy(AttackArrow.gameObject);
    }

    public void ShowIntent() {
        _nextAction = GetAction();
        UpdateVisuals();
    }

    private EnemyAction GetAction() {
        switch(EnemyType) {
            case EnemyType.Goblin: {
                return new EnemyAction(EnemyActionType.Attack, Controller.GetValidAttackTarget(CombatSlot));
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
        var euler = transform.localEulerAngles;
        transform.LookAt(_nextAction.TargetSlot.transform.position);
        euler.y = transform.localEulerAngles.y;
        transform.localEulerAngles = euler;
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
        CombatSlot newCombatSlot = Controller.SpawnBlockSlot(blockPos);

        newCombatSlot.AttackArrow = AttackArrow;
        AttackArrow.BlockSlot = newCombatSlot;
        AttackArrow.Owner = this;
    }

    public void SetTargetSlot(CombatSlot newTarget)
    {
        _nextAction.TargetSlot = newTarget;
        UpdateVisuals();
    }
}