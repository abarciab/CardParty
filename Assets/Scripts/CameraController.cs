using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private Vector3 _tileOffset = new Vector3(0, 4.1f, -4.5f);
    private Vector3 _currentTilePos;
    [SerializeField] private float _moveTime = 1.5f;
    [SerializeField] private AnimationCurve _moveCurve;

    [Header("Timing")]
    [SerializeField] private float _wipeDelay = 0.5f;
    [SerializeField] private float _wipeDuration = 0.5f;

    private Vector3 _lookTarget;
    private bool _isLookingAtPlayer;

    private void Start()
    {
        _player = OverworldManager.i.Player.transform;
        _currentTilePos = transform.position - _tileOffset;
        _isLookingAtPlayer = true;
    }

    void Update()
    {
        if (_isLookingAtPlayer) LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        _lookTarget = Vector3.Lerp(_lookTarget, _player.position, 7.5f * Time.deltaTime);
        transform.LookAt(_lookTarget);
    }

    public async void MoveToFocusOnNewTile(Vector3 tilePos)
    {
        var dist = Vector3.Distance(_currentTilePos, tilePos);
        if (dist < 0.01f) return;

        _isLookingAtPlayer = false;
        await Task.Delay((int)(_wipeDelay * 1000));

        transform.position = _currentTilePos + _tileOffset;

        _currentTilePos = tilePos;
        StopAllCoroutines();
        StartCoroutine(AnimateToNewTile());
        await OverworldUIManager.i.WipeScreen(_wipeDuration);
        _isLookingAtPlayer = true;
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
