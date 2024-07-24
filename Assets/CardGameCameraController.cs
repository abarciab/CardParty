using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameCameraController : MonoBehaviour
{
    private bool _isOverhead;
    [SerializeField] private Transform _rear;
    [SerializeField] private Transform _overhead;
    [SerializeField] private Light _sun;

    public void Toggle()
    {
        if (_isOverhead) SwitchToRear();
        else SwitchToOverhead();
    }

    public void SwitchToOverhead()
    {
        Set(_overhead);
        _isOverhead = true;
        _sun.shadows = LightShadows.None;
    }

    public void SwitchToRear()
    {
        Set(_rear);
        _isOverhead = false;
        _sun.shadows = LightShadows.Soft;
    }

    private void Set(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
