using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour
{
    [SerializeField] private float _amplitude = 0.005f;
    [SerializeField] private float _frequency = 5;
    [SerializeField] private bool _randomStart;
    private float _offset;
    [SerializeField] private bool _fixedTime;

    private void Start()
    {
        if (_randomStart) _offset = Random.Range(-10, 10);
        _amplitude *= Random.Range(0.9f, 1.1f);
        _frequency *= Random.Range(0.9f, 1.1f);
    }

    void Update()
    {
        float time = _fixedTime ? Time.fixedTime : Time.time;
        transform.localPosition += _amplitude * Mathf.Sin((time + _offset) * _frequency) * Vector3.up;
    }
}
