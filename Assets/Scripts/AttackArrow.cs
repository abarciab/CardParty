using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class AttackArrow : MonoBehaviour
{
    private const float BUFFER_DIST = 1.5f;
    private const float HEAD_SIZE = 2f;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _tail;
    [SerializeField] private Transform _damageCanvas;
    [SerializeField] private TextMeshProUGUI _damageText;

    [HideInInspector] public AdventurerObject TargetAdventurer => (AdventurerObject) (BlockSlot.Creature ? BlockSlot.Creature : _mainSlot.Creature);
    [HideInInspector] public CombatSlot BlockSlot { get; private set; }
    [HideInInspector] public EnemyObject Owner { get; private set; }

    private Vector3 _startPos => Owner.transform.position;
    private Vector3 _endPos => BlockSlot && BlockSlot.Creature ? BlockSlot.transform.position : _mainSlot.transform.position;
    private CombatSlot _mainSlot;


    public void Initialize(EnemyObject owner, CombatSlot targetSlot, int damage) {

        _damageText.text = damage.ToString();

        Owner = owner;
        _mainSlot = targetSlot;
        _mainSlot.Arrows.Add(this);

        BlockSlot = owner.SpawnBlockSlot(Vector3.Lerp(_startPos, _endPos, 0.5f));
        BlockSlot.Arrows.Add(this);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        transform.position = Vector3.Lerp(_startPos, _endPos, 0.75f);
        transform.position += Vector3.up * 2;

        float distance = Vector3.Distance(_startPos, _endPos);
        _tail.transform.localScale = new Vector3(_tail.transform.localScale.x, _tail.transform.localScale.y, distance - (3 * BUFFER_DIST));

        var euler = transform.localEulerAngles;
        transform.LookAt(_endPos);
        euler.y = transform.localEulerAngles.y;
        transform.localEulerAngles = euler;

        BlockSlot.IncomingAttacks.Remove(this);
        _mainSlot.IncomingAttacks.Remove(this);
        if (BlockSlot.Creature && !_mainSlot.Creature) BlockSlot.IncomingAttacks.Add(this);
        if (!BlockSlot.Creature) _mainSlot.IncomingAttacks.Add(this);


        var pos = Vector3.Lerp(_startPos, _endPos, 0.6f);
        pos.y = _damageCanvas.transform.position.y;
        _damageCanvas.transform.position = pos;
    }

    private void OnDestroy()
    {
        if (BlockSlot) BlockSlot.Arrows.Remove(this);
        if (_mainSlot) _mainSlot.Arrows.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Owner) return;
        Gizmos.DrawLine(_startPos, _endPos);
        Gizmos.DrawWireSphere(_startPos, 0.2f);
    }
}