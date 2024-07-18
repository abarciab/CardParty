using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OverworldPlayer : MonoBehaviour
{

    [SerializeField] private bool _showDebug;

    [Header("Movement")]
    [SerializeField] private LayerMask _walkableLayers;
    [SerializeField] private float _maxSpeed = 10;
    [SerializeField] private float _minSpeed = 2;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField, ConditionalField(nameof(_showDebug))] private float _distThreshold = 0.05f;
    [SerializeField] private Transform _model;
    [SerializeField] private float _turnLerpFactor = 5;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _walkingAnimBool = "walking";

    [Header("Misc")]
    [SerializeField] private Transform _walkTargetVisual;


    [Header("Sounds")]
    [SerializeField] private Sound _playerMoveSound;
    [SerializeField] private Sound _moveTileSound;

    private Vector3 _currentTarget;
    private float _speed;
    private float _originalDistanceToTarget;
    private float _distanceToTarget;
    private TileController _currentTile;
    private bool _isBeingControlled;
    private UnityAction _callbackOnTargetReached;

    private void Start()
    {
        _playerMoveSound = Instantiate(_playerMoveSound);
        _moveTileSound = Instantiate(_moveTileSound);

        _currentTarget = transform.position;
    }

    private void Update()
    {
        if (!_isBeingControlled && (UIManager.i.IsBusy || EventSystem.current.IsPointerOverGameObject())) {
            Stop();
            return;
        }

        SetDistanceToTarget();
        if (!_isBeingControlled && !UIManager.i.IsBusy) CheckForNewWalkTarget();
        CalculateSpeed();
        bool notAtTarget = _distanceToTarget > _distThreshold;
        _animator.SetBool(_walkingAnimBool, notAtTarget);
        _walkTargetVisual.gameObject.SetActive(notAtTarget);
        if (notAtTarget) MoveTowardTarget();
        else if (_isBeingControlled) FinishControl();
    }

    private void Stop()
    {
        _animator.SetBool(_walkingAnimBool, false);
        _currentTarget = transform.position;
    }

    public TileController GetCurrentTile() => _currentTile;

    private void FinishControl()
    {
        if (_callbackOnTargetReached != null) _callbackOnTargetReached.Invoke();
        _callbackOnTargetReached = null;
        _isBeingControlled = false;
    }

    public void MoveToTargetWithCallback(Vector3 newTarget, UnityAction callback)
    {
        _playerMoveSound.Play();
        _isBeingControlled = true;
        newTarget.y = transform.position.y;
        _currentTarget = newTarget;
        _callbackOnTargetReached = callback;
    }

    public void MoveToNewTile(TileController newTile, Direction dir)
    {
        _moveTileSound.Play();
        var entrancePos = newTile.GetEntrancePos(dir);
        entrancePos.y = transform.position.y;
        transform.position = entrancePos;
        _currentTarget = transform.position;
        SetCurrentTile(newTile);
        OverworldManager.i.OnNewTileEntered.Invoke();
    }

    public void SetCurrentTile(TileController newTile)
    {
        _currentTile = newTile;
        OverworldManager.i.CameraController.MoveToFocusOnNewTile(newTile.transform.position);
    }

    private void CalculateSpeed()
    {
        float progress = _distanceToTarget / Mathf.Max(_originalDistanceToTarget, 0.001f);
        float speedMod = _speedCurve.Evaluate(progress);
        _speed = speedMod * (_maxSpeed - _minSpeed) + _minSpeed;
    }

    private void SetDistanceToTarget()
    {
        _distanceToTarget = Mathf.Max(_distThreshold / 2, Vector3.Distance(transform.position, _currentTarget));
    }

    private float GetDistanceToTarget()
    {
        SetDistanceToTarget();
        return _distanceToTarget;
    }

    private void CheckForNewWalkTarget()
    {
        if (!Input.GetMouseButtonDown(0)) return;


        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitPoint = Physics.Raycast(mouseRay, out var hitData, 300, _walkableLayers);
        if (!hitPoint) return;

        //print("recieved click, but no tile");

        var tile = hitData.collider.GetComponentInParent<TileController>();
        if (tile != _currentTile) return;

        //print("recieved click");

        _walkTargetVisual.position = hitData.point;
        _playerMoveSound.Play();
        _currentTarget = hitData.point;
        _originalDistanceToTarget = GetDistanceToTarget();
    }

    private void MoveTowardTarget()
    {
        var dir = (_currentTarget - transform.position).normalized;
        var delta = _speed * Time.deltaTime * dir;
        transform.position += delta;
        FaceMoveDir(delta.normalized);
        PlaceModelOnGround();
    }

    private void PlaceModelOnGround()
    {
        Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out var hitInfo, _walkableLayers, 200);
        var pos = _model.transform.position;
        pos.y = hitInfo.point.y;
        _model.transform.position = pos;
    }

    private void FaceMoveDir(Vector3 normalizedDelta)
    {
        var euler = _model.localEulerAngles;
        var target = _model.localEulerAngles;
        _model.LookAt(transform.position + normalizedDelta);
        target.y = _model.localEulerAngles.y;
        var newRot = Quaternion.Lerp(Quaternion.Euler(euler), Quaternion.Euler(target), _turnLerpFactor * Time.deltaTime);
        _model.localRotation = newRot;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showDebug) return;

        Gizmos.DrawWireSphere(_currentTarget, _distThreshold);
    }
}
