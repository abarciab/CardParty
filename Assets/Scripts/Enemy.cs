using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

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
    public IEnumerator Action(List<Adventurer> adventurers, List<Enemy> enemies) {
        switch(_nextAction.Action) {
            case EnemyActionType.Attack: {
                Adventurer target = (Adventurer)_nextAction.TargetSlot.Creature;
                if (AttackArrow.BlockSlot.Creature) target = (Adventurer)AttackArrow.BlockSlot.Creature;

                yield return StartCoroutine(Utilities.LerpToAndBack(gameObject, target.transform.position));
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
                return new EnemyAction(EnemyActionType.Attack, CardGameManager.i.GetRandomAdventurerSlot());
            }
            break;
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
            AttackArrow = GameObject.Instantiate(_attackArrowPrefab, transform.parent).GetComponent<AttackArrow>();
            AttackArrow.SetArrow(transform.position, _nextAction.TargetSlot.transform.position);

            CombatSlot newCombatSlot = CardGameManager.i.SpawnBlockSlot(AttackArrow.transform.position);

            newCombatSlot.AttackArrow = AttackArrow;
            AttackArrow.BlockSlot = newCombatSlot;
            AttackArrow.Owner = this;
        } else {
            AttackArrow.SetArrow(transform.position, _nextAction.TargetSlot.transform.position);
        }
    }
}