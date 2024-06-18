using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPlayer : MonoBehaviour
{

    [SerializeField] private bool _showDebug;

    [Header("Movement")]
    [SerializeField] private LayerMask _walkableLayers;
    [SerializeField] private float _maxSpeed = 10;
    [SerializeField] private float _minSpeed = 2;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField, ConditionalField(nameof(_showDebug))] private float _distThreshold = 0.05f;

    private Vector3 _currentTarget;
    private float _speed;
    private float _originalDistanceToTarget;
    private float _distanceToTarget;

    private void Start()
    {
        _currentTarget = transform.position;
    }

    private void Update()
    {
        CheckForNewWalkTarget();
        CalculateSpeed();
        if (DistanceToTarget() > _distThreshold) MoveTowardTarget();
    }

    private void CalculateSpeed()
    {
        float progress = _distanceToTarget / _originalDistanceToTarget;
        float speedMod = _speedCurve.Evaluate(progress);
        _speed = speedMod * (_maxSpeed - _minSpeed) + _minSpeed;
    }

    private float DistanceToTarget()
    {
        _distanceToTarget = Mathf.Max(_distThreshold /2,  Vector3.Distance(transform.position, _currentTarget));
        return _distanceToTarget;
    }

    private void CheckForNewWalkTarget()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitPoint = Physics.Raycast(mouseRay, out var hitData, 300, _walkableLayers);
        if (!hitPoint) return;

        var tile = hitData.collider.GetComponentInParent<TileController>();
        if (tile) OverworldManager.i.CameraController.MoveToFocusOnNewTile(tile.transform.position);
        _currentTarget = hitData.point;
        _originalDistanceToTarget = DistanceToTarget();
    }

    private void MoveTowardTarget()
    {
        var dir = (_currentTarget - transform.position).normalized;
        transform.position += _speed * Time.deltaTime * dir;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showDebug) return;

        Gizmos.DrawWireSphere(_currentTarget, _distThreshold);
    }
}
