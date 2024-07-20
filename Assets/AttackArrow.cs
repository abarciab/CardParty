using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArrow : MonoBehaviour
{
    private const float BUFFER_DIST = 1.5f;
    private const float HEAD_SIZE = 2f;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject tail;
    public CombatSlot BlockSlot;
    public EnemyObject Owner;

    private Vector3 _posA;
    private Vector3 _posB;

    public void Initialize(Vector3 posA, Vector3 posB) {
        _posA = posA;
        _posB = posB;

        transform.position = GetPosAlongLine(0.75f);

        var euler = transform.localEulerAngles;
        transform.LookAt(posB);
        euler.y = transform.localEulerAngles.y;
        transform.localEulerAngles = euler;

        float distance = Vector3.Distance(posA, posB);
        tail.transform.localScale = new Vector3(tail.transform.localScale.x, tail.transform.localScale.y, distance - (3 * BUFFER_DIST));
    }

    private Vector3 GetPosAlongLine(float percent)
    {
        return Vector3.Lerp(_posA, _posB, percent);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(_posA, _posB);
        Gizmos.DrawWireSphere(_posA, 0.2f);
    }
}