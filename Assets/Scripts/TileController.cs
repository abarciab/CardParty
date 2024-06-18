using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class TileController : MonoBehaviour
{
    [SerializeField] private GameObject _interactable;
    private TileGridController _gridController;
    public Vector2Int ID { get; private set; }
    [SerializeField] private List<EntranceData> _entraces = new List<EntranceData>();
    private bool _isUnlocked;
    private Direction _initialEntranceDir;


    private void OnValidate()
    {
        foreach (var e in _entraces) e.Name = e.Dir.ToString();
    }

    public void Initialize(int x, int y, bool _isMiddle, TileGridController gridController)
    {
        ID = new Vector2Int(x, y);
        _gridController = gridController;
        gameObject.name = "tile (" + x + ", " + y + ")" + (_isMiddle ? "(Middle)" : "");
        if (_isMiddle) OverworldManager.i.Player.SetCurrentTile(this);

        if (_isMiddle) {
            ShowAllEntrances();
            _interactable.SetActive(false);
        }
        else HideAllEntrances();
    }

    private void ShowAllEntrances()
    {
        _isUnlocked = true;
        foreach (var e in _entraces) e.Door.SetActive(true);
    }

    private void HideAllEntrances()
    {
        foreach (var e in _entraces) e.Door.SetActive(false);
    }

    public void EnterTile(Direction entranceDir)
    {
        OverworldManager.i.Player.MoveToNewTile(this, entranceDir);
        _initialEntranceDir = entranceDir;
        UpdateEntranceVisuals();
    }

    private void UpdateEntranceVisuals()
    {
        foreach (var e in _entraces) e.Door.SetActive(_isUnlocked || e.Dir == _initialEntranceDir); 
    }

    public void ClickOnInteractable()
    {
        OverworldManager.i.LoadCardGame();
        _isUnlocked = true;
    }

    private void OnEnable()
    {
        UpdateEntranceVisuals();
        _interactable.SetActive(!_isUnlocked);
    }

    public Vector3 GetEntrancePos(Direction dir)
    {
        return _entraces.Where(x => x.Dir == dir).First().TpPoint.position;
    }


    public void PressUp() => PressNavigationButton(Direction.Up);
    public void PressRight() => PressNavigationButton(Direction.Right);
    public void PressDown() => PressNavigationButton(Direction.Down);
    public void PressLeft() => PressNavigationButton(Direction.Left);

    private void PressNavigationButton(Direction dir)
    {
        if (!_isUnlocked && dir != _initialEntranceDir) return;
        var nextTile = _gridController.GetTileInDirection(ID, dir);
        if (nextTile != null) MovePlayerToTile(nextTile, dir);
    }

    private void MovePlayerToTile(TileController nextTile, Direction exitDir)
    {
        Direction entranceDir = (Direction)(((int)exitDir + 2) % 4);
        nextTile.EnterTile(entranceDir);
    }
}
