using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

[SelectionBase]
public class TileController : MonoBehaviour
{
    public WFCSTileInfo WFCInfo;
    public Vector2Int GridPos { get; private set; }

    [SerializeField] private List<EntranceData> _entraces = new List<EntranceData>();

    private TileGridController _gridController;
    private TileInteractable _interactable;
    private Direction _initialEntranceDir;
    private bool _isUnlocked;

    private void OnValidate()
    {
        foreach (var e in _entraces) e.Name = e.Dir.ToString();
    }

    private void OnEnable()
    {
        UpdateEntranceVisuals();
        if (!_interactable) _interactable = GetComponentInChildren<TileInteractable>();
        _interactable.gameObject.SetActive(!_isUnlocked);
    }

    private void Start()
    {
        _interactable.gameObject.SetActive(false);
    }

    [ButtonMethod]
    private void FixDataTile()
    {
        WFCInfo = GetComponentInChildren<WFCSTileInfo>();
        var transf = WFCInfo.transform;
        transf.localPosition = new Vector3(0, -2.87f, 0);
        transf.localScale = Vector3.one * 3;
    }

    public void Initialize(int x, int y, bool _isCenter, TileGridController gridController, Quaternion rot, TileInteractableData interactableData)
    {
        GridPos = new Vector2Int(x, y);
        _gridController = gridController;
        gameObject.name = "tile (" + x + ", " + y + ")" + (_isCenter ? "(Middle)" : "");
        if (_isCenter) OverworldManager.i.Player.SetCurrentTile(this);

        if (_isCenter) {
            ShowAllEntrances();
            _interactable.gameObject.SetActive(false);
        }
        else HideAllEntrances();

        SetRotation(rot);
        WFCInfo.Rotate(rot);
        _interactable.Initialize(interactableData, this, rot);
    }

    private void SetRotation(Quaternion rot)
    {
        if (rot == Quaternion.identity) return;
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 90, 0)) < 0.1f) RotateEntrances(1);
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 180, 0)) < 0.1f) RotateEntrances(2);
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 270, 0)) < 0.1f) RotateEntrances(3);

        _entraces[0].Dir = Direction.UP;
        _entraces[1].Dir = Direction.RIGHT;
        _entraces[2].Dir = Direction.DOWN;
        _entraces[3].Dir = Direction.LEFT;
    }

    private void RotateEntrances(int numTimes)
    {
        for (int i = 0; i < numTimes; i++) {
            _entraces.Insert(0, _entraces[3]);
            _entraces.RemoveAt(4);
        }
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
        _gridController.UpdateAllTiles(this);
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
    }

    public void UpdateEntranceVisuals()
    {
        if (OverworldManager.i.Player.GetCurrentTile() != this) HideAllEntrances();
        else foreach (var e in _entraces) e.Door.SetActive(_isUnlocked || e.Dir == _initialEntranceDir); 
    }

    public void ClickOnInteractable()
    { 
        _isUnlocked = true;

        var pos = _interactable.GetCurrentObjPos();
        var outcome = _interactable.Data.Outcome;

        if (outcome == TileInteractableOutcome.FIGHT) OverworldManager.i.Player.MoveToTargetWithCallback(pos, StartFightFromInteractable);
        if (outcome == TileInteractableOutcome.EVENT) OverworldManager.i.Player.MoveToTargetWithCallback(pos, StartEventFromInteractable);
        if (outcome == TileInteractableOutcome.SHOP) OverworldManager.i.Player.MoveToTargetWithCallback(pos, OpenShopFromInteractable);
    }

    private void StartFightFromInteractable()
    {
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
        OverworldManager.i.LoadCardGame();
    }

    private void OpenShopFromInteractable()
    {
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
        OverworldUIManager.i.OpenShop();
    }

    private void StartEventFromInteractable()
    {
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
        OverworldUIManager.i.StartRandomEvent();
    }

    public Vector3 GetEntrancePos(Direction dir)
    {
        return _entraces.Where(x => x.Dir == dir).First().TpPoint.position;
    }

    public void PressButton(GameObject door)
    {
        foreach (var e in _entraces) if (e.Door == door) PressNavigationButton(e.Dir);
    }

    private void PressNavigationButton(Direction dir)
    {
        if (!_isUnlocked && dir != _initialEntranceDir) return;
        var nextTile = _gridController.GetTileInDirection(GridPos, dir);
        if (nextTile != null) {
            var exitPos = _entraces.Where(x => x.Dir == dir).First().Door.transform.position;
            OverworldManager.i.Player.MoveToTargetWithCallback(exitPos, () => MovePlayerToTile(nextTile, dir));
        }
    }

    private void MovePlayerToTile(TileController nextTile, Direction exitDir)
    {
        Direction entranceDir = (Direction)(((int)exitDir + 2) % 4);
        nextTile.EnterTile(entranceDir);
    }
}
