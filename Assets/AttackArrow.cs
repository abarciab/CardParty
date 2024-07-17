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
    public Enemy Owner;
    public void SetArrow(Vector3 posA, Vector3 posB) {
        transform.position = posA;
        head.transform.localPosition = Vector3.zero;

        Vector3 vecTowards = posB - posA;

        transform.rotation = Quaternion.LookRotation(posB);
        transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.position += transform.forward * vecTowards.magnitude / 2;

        tail.transform.localScale = new Vector3(tail.transform.localScale.x, tail.transform.localScale.y, vecTowards.magnitude - (2 * BUFFER_DIST));

        head.transform.position += (transform.forward * vecTowards.magnitude / 2) - (transform.forward * (HEAD_SIZE));
    }
}