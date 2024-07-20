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

public class Enemy : Creature
{
    public EnemyType EnemyType;
    private EnemyAction _nextAction;
    [SerializeField] private GameObject _attackArrowPrefab;
    public AttackArrow AttackArrow;
    [SerializeField] private float _attackDamage;
    public async Task Action(List<Adventurer> adventurers, List<Enemy> enemies) {
        if (_isStunned) return;
        switch(_nextAction.Action) {
            case EnemyActionType.Attack: {
                Adventurer target = (Adventurer)_nextAction.TargetSlot.Creature;
                    if (target == null) break;
                if (AttackArrow.BlockSlot.Creature) target = (Adventurer)AttackArrow.BlockSlot.Creature;

                await Utilities.LerpToAndBack(gameObject, target.transform.position);
                target.TakeDamage(_attackDamage);
            }
            break;
        }

        AttackArrow.BlockSlot.MoveCreature();
        Destroy(AttackArrow.BlockSlot.gameObject);
        Destroy(AttackArrow.gameObject);
    }

    public void ShowIntent(List<Adventurer> adventurers, List<Enemy> enemies) {
        _nextAction = GetAction();
        SetTargetSlot(_nextAction.TargetSlot);
    }

    private EnemyAction GetAction() {
        switch(EnemyType) {
            case EnemyType.Goblin: {
                return new EnemyAction(EnemyActionType.Attack, Controller.GetRandomAdventurerSlot());
            }
        }

        return new EnemyAction(EnemyActionType.None, null);
    }

    public CombatSlot GetTarget() {
        return _nextAction.TargetSlot;
    }

    public void SetTargetSlot(CombatSlot newTargetSlot) {
        if (!_nextAction.TargetSlot) return;

        _nextAction.TargetSlot = newTargetSlot;

        if (!AttackArrow) {
            AttackArrow = Instantiate(_attackArrowPrefab, transform.parent).GetComponent<AttackArrow>();
            var arrowStart = transform.position;
            var arrowEnd = _nextAction.TargetSlot.transform.position;
            AttackArrow.SetArrow(arrowStart, arrowEnd);

            var blockPos = Vector3.Lerp(arrowStart, arrowEnd, 0.5f);
            CombatSlot newCombatSlot = Controller.SpawnBlockSlot(blockPos);

            newCombatSlot.AttackArrow = AttackArrow;
            AttackArrow.BlockSlot = newCombatSlot;
            AttackArrow.Owner = this;
        } else {
            AttackArrow.SetArrow(transform.position, _nextAction.TargetSlot.transform.position);
        }
    }
}