using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class TileController : MonoBehaviour
{
    [SerializeField] private GameObject _interactable;
    private TileGridController _gridController;
    public Vector2Int ID { get; private set; }
    [SerializeField] private Transform _upEntranceLoc;
    [SerializeField] private Transform _rightEntranceLoc;
    [SerializeField] private Transform _downEntranceLoc;
    [SerializeField] private Transform _leftEntranceLoc;

    private void Start()
    {
        if (Random.Range(0, 1f) < 0.5f) _interactable.SetActive(false);
    }

    public void ClickOnInteractable()
    {
        OverworldManager.i.LoadCardGame();
    }

    public Vector3 GetEntrancePos(Direction dir)
    {
        if (dir == Direction.Up) return _upEntranceLoc.position;
        if (dir == Direction.Right) return _rightEntranceLoc.position;
        if (dir == Direction.Down) return _downEntranceLoc.position;
        if (dir == Direction.Left) return _leftEntranceLoc.position;
        return Vector3.zero;
    }

    public void Initialize(int x, int y, bool _isMiddle, TileGridController gridController)
    {
        ID = new Vector2Int(x, y);
        _gridController = gridController;
        gameObject.name = "tile (" + x + ", " + y + ")" + (_isMiddle ? "(Middle)" : "");
        if (_isMiddle) OverworldManager.i.Player.SetCurrentTile(this);
    }

    public void PressUp() => PressNavigationButton(Direction.Up);
    public void PressRight() => PressNavigationButton(Direction.Right);
    public void PressDown() => PressNavigationButton(Direction.Down);
    public void PressLeft() => PressNavigationButton(Direction.Left);

    private void PressNavigationButton(Direction dir)
    {
        var nextTile = _gridController.GetTileInDirection(ID, dir);
        if (nextTile != null) MovePlayerToTile(nextTile, dir);
    }

    private void MovePlayerToTile(TileController nextTile, Direction exitDir)
    {
        Direction entranceDir = (Direction)(((int)exitDir + 2) % 4);

        OverworldManager.i.Player.MoveToNewTile(nextTile, entranceDir);
    }
}
