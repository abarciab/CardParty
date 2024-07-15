using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private Vector3 _axis = Vector3.forward;
    [SerializeField] private bool _constant = true;
    [SerializeField] private float _speed;
    [SerializeField, ConditionalField(nameof(_constant), inverse: true)] private float _amplitude = 0.01f;
    [SerializeField] private bool _randomStart;

    private void Start()
    {
        if (!_randomStart) return;
        if (_constant) transform.localEulerAngles += _axis * Random.Range(0, 360);
        else {
            _amplitude *= Random.Range(0.9f, 1.1f);
            _speed *= Random.Range(0.5f, 1.5f);
        }
    }

    private void Update()
    {
        if (_constant) transform.localEulerAngles += _axis * _speed * Time.deltaTime;
        else {
            //var delta = Mathf.Sin(Time.time * _speed) * Mathf.Abs(_limits.y - _limits.x) - _limits.x;
            var delta = Mathf.Sin(Time.time * _speed) * _amplitude;
            transform.Rotate(_axis, delta);
        }
    }
}
