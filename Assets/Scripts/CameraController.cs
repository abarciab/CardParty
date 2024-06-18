using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private Vector3 _tileOffset = new Vector3(0, 4.1f, -4.5f);
    private Vector3 _currentTilePos;
    [SerializeField] private float _moveTime = 1.5f;
    [SerializeField] private AnimationCurve _moveCurve;

    private void Start()
    {
        _player = OverworldManager.i.Player.transform;
        _currentTilePos = transform.position - _tileOffset;
    }

    void Update()
    {
        transform.LookAt(_player);
    }

    public void MoveToFocusOnNewTile(Vector3 tilePos)
    {
        var dist = Vector3.Distance(_currentTilePos, tilePos);
        if (dist < 0.01f) return;

        transform.position = _currentTilePos + _tileOffset;

        _currentTilePos = tilePos;
        StopAllCoroutines();
        StartCoroutine(AnimateToNewTile());
    }

    private IEnumerator AnimateToNewTile()
    {
        var startPos = transform.position;
        var targetPos = _currentTilePos + _tileOffset;
        float timePassed = 0;
        while (timePassed < _moveTime) {
            float progress = timePassed / _moveTime;
            progress = _moveCurve.Evaluate(progress);
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = targetPos;
    }
}
