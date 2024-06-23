using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class TileController : MonoBehaviour
{
    [SerializeField] private GameObject _interactable;
    public WFCSTileInfo WFCInfo;

    private TileGridController _gridController;
    public Vector2Int GridPos { get; private set; }
    [SerializeField] private List<EntranceData> _entraces = new List<EntranceData>();
    private bool _isUnlocked;
    private Direction _initialEntranceDir;

    private void OnValidate()
    {
        foreach (var e in _entraces) e.Name = e.Dir.ToString();
    }

    private void Start()
    {
        _interactable.SetActive(false);
        FixEntranceButtons();
    }

    [ButtonMethod]
    private void FixEntranceButtons()
    {
        var doors = GetComponentsInChildren<OnClickOnCollider>();
        foreach (var d in doors)
        {
            if (d.name.Contains("able")) continue;
            d.OverrideOnClickOn(() => PressButton(d.gameObject));
        }
    }

    [ButtonMethod]
    private void FixDataTile()
    {
        WFCInfo = GetComponentInChildren<WFCSTileInfo>();
        var transf = WFCInfo.transform;
        transf.localPosition = new Vector3(0, -2.87f, 0);
        transf.localScale = Vector3.one * 3;
    }

    public void Initialize(int x, int y, bool _isMiddle, TileGridController gridController, Quaternion rot)
    {
        GridPos = new Vector2Int(x, y);
        _gridController = gridController;
        gameObject.name = "tile (" + x + ", " + y + ")" + (_isMiddle ? "(Middle)" : "");
        if (_isMiddle) OverworldManager.i.Player.SetCurrentTile(this);

        if (_isMiddle) {
            ShowAllEntrances();
            _interactable.SetActive(false);
        }
        else HideAllEntrances();

        SetRotation(rot);
        WFCInfo.Rotate(rot);
    }

    private void SetRotation(Quaternion rot)
    {
        if (rot == Quaternion.identity) return;
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 90, 0)) < 0.1f) RotateEntrances(1);
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 180, 0)) < 0.1f) RotateEntrances(2);
        if (Vector3.Distance(rot.eulerAngles, new Vector3(0, 270, 0)) < 0.1f) RotateEntrances(3);

        _entraces[0].Dir = Direction.Up;
        _entraces[1].Dir = Direction.Right;
        _entraces[2].Dir = Direction.Down;
        _entraces[3].Dir = Direction.Left;
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
        _interactable.SetActive(!_isUnlocked);
    }

    public void UpdateEntranceVisuals()
    {
        if (OverworldManager.i.Player.GetCurrentTile() != this) HideAllEntrances();
        else foreach (var e in _entraces) e.Door.SetActive(_isUnlocked || e.Dir == _initialEntranceDir); 
    }

    public void ClickOnInteractable()
    {
        _isUnlocked = true;

        //OverworldManager.i.Player.MoveToTargetWithCallback(_interactable.transform.position, () =>OverworldManager.i.LoadCardGame());
        //OverworldManager.i.Player.MoveToTargetWithCallback(_interactable.transform.position, StartEventFromInteractable);
        OverworldManager.i.Player.MoveToTargetWithCallback(_interactable.transform.position, OpenShopInteractable);
    }

    private void OpenShopInteractable()
    {
        OverworldUIManager.i.OpenShop();
        UpdateEntranceVisuals();
        _interactable.SetActive(!_isUnlocked);
    }

    private void StartEventFromInteractable()
    {
        OverworldUIManager.i.StartRandomEvent();
        UpdateEntranceVisuals();
        _interactable.SetActive(!_isUnlocked);
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
