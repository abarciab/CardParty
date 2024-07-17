using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class TileController : MonoBehaviour
{
    public WFCSTileInfo WFCInfo;
    public Vector2Int GridPos { get; private set; }

    [SerializeField] private List<Sprite> _mapSprites = new List<Sprite>();

    [SerializeField] private List<EntranceData> _entraces = new List<EntranceData>();
    public List<EntranceData> Entrances => _entraces;

    private TileGridController _gridController;
    private TileInteractable _interactable;
    private Direction _initialEntranceDir;
    private bool _isUnlocked;

    [SerializeField, ReadOnly] private int _turns; 

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

    public void ShowOnMap()
    {
        int turns = Utilities.QuaternionToTurnCount(transform.rotation);
        OverworldUIManager.i.RevealMapSprite(GridPos, _mapSprites[Random.Range(0, _mapSprites.Count)], turns);
        _turns = turns;
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

            ShowOnMap();
            OverworldUIManager.i.EnterTileOnMap(GridPos);
        }
        else HideAllEntrances();

        SetRotation(rot);
        WFCInfo.Rotate(rot);
        _interactable.Initialize(interactableData, this, rot);
    }

    private void SetRotation(Quaternion rot)
    {
        var turnCount = Utilities.QuaternionToTurnCount(rot);
        RotateEntrances(turnCount);

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

        OverworldUIManager.i.EnterTileOnMap(GridPos);
        ShowOnMap();
    }

    public void UpdateEntranceVisuals()
    {
        if (OverworldManager.i.Player.GetCurrentTile() != this) HideAllEntrances();
        else foreach (var e in _entraces) e.Door.SetActive(_isUnlocked || e.Dir == _initialEntranceDir); 
    }

    public void ClickOnInteractable(TileInteractableData data)
    {
        _isUnlocked = true;

        var pos = _interactable.GetCurrentObjPos();
        var outcome = data.Outcome;
        UnityAction callback = null;
        var player = OverworldManager.i.Player;

        if (outcome == TileInteractableOutcome.FIGHT) callback = StartFightFromInteractable;
        if (outcome == TileInteractableOutcome.EVENT) callback = StartEventFromInteractable;
        if (outcome == TileInteractableOutcome.SHOP) callback = () => OpenShopFromInteractable(data.ShopData);

        player.MoveToTargetWithCallback(pos, callback);
    }

    private void StartFightFromInteractable()
    {
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
        OverworldManager.i.LoadCardGame();
    }

    private void OpenShopFromInteractable(ShopData data)
    {
        UpdateEntranceVisuals();
        //_interactable.gameObject.SetActive(!_isUnlocked);
        OverworldUIManager.i.OpenShop(data);
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
