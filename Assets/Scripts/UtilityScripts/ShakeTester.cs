using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class ShakeTester : MonoBehaviour
{
    [SerializeField] private CameraShake _shake;

    [SerializeField, Range(0, 1)] private float _speed;
    [SerializeField, Range(0, 1)] private float _amplitude;


    [SerializeField] private float _unrestrictedSpeed;
    [SerializeField] private float _unrestrictedAmplitude;

    [SerializeField] private float _duration;

    [ButtonMethod]
    private void DefaultShake()
    {
        _shake.ShakeDefault();
    }

    [ButtonMethod]
    private void ShakeManual()
    {
        _shake.ShakeManual(_speed, _amplitude, _duration);
    }

    [ButtonMethod]
    private void ShakeUnrestricted()
    {
        _shake.ShakeUnrestricted(_unrestrictedSpeed, _unrestrictedAmplitude, _duration);
    }
}
