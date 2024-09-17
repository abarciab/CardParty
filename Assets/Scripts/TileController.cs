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

    [SerializeField] private GameObject _decorationParent;

    private TileGridController _gridController;
    private TileInteractable _interactable;
    private Direction _initialEntranceDir;
    private bool _isUnlocked;
    private bool _isCenter;
    private bool _isWin;

    [SerializeField, ReadOnly] private int _turns;
    [SerializeField, ReadOnly] private string _interactableName;
    [SerializeField, ReadOnly, Range(0, 1)] private int _difficulty;

    private bool _isActive => OverworldManager.i.Player.GetCurrentTile() == this;

    private void OnValidate()
    {
        foreach (var e in _entraces) e.Name = e.Dir.ToString();
    }

    private void OnEnable()
    {
        UpdateVisuals();
    }

    private void Start()
    {
        _interactable.gameObject.SetActive(false);
        UpdateVisuals();

    }

    private void UpdateVisuals()
    {
        UpdateEntranceVisuals();
        if (!_interactable) _interactable = GetComponentInChildren<TileInteractable>();
        _interactable.gameObject.SetActive(!_isUnlocked && _isActive);
        _decorationParent.SetActive(_isActive);
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

    public void Initialize(int x, int y, bool isCenter, bool isWin, TileGridController gridController, Quaternion rot, TileInteractableData interactableData, int difficulty)
    {
        _isWin = isWin;
        _isCenter = isCenter;

        GridPos = new Vector2Int(x, y);
        _gridController = gridController;
        gameObject.name = "tile (" + x + ", " + y + ")" + (_isCenter ? "(Middle)" : "");
        if (_isCenter) OverworldManager.i.Player.SetCurrentTile(this);

        if (_isCenter) {
            _isUnlocked = true;
            ShowAllEntrances();
            _interactable.gameObject.SetActive(false);

            ShowOnMap();
            OverworldUIManager.i.EnterTileOnMap(GridPos);
        }
        else HideAllEntrances();

        SetRotation(rot);
        WFCInfo.Rotate(rot);
        _interactable.Initialize(interactableData, this, rot);
        _interactableName = interactableData.Name;
        _difficulty = difficulty;

        UpdateVisuals();
        if (isWin) ShowOnMap();
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
        UpdateVisuals();
    }

    public void UpdateEntranceVisuals()
    {
        if (!_isActive) HideAllEntrances();
        else foreach (var e in _entraces) e.Door.SetActive(_isUnlocked || e.Dir == _initialEntranceDir); 
    }

    public void ClickOnInteractable(TileInteractableData data)
    {
        _isUnlocked = true;

        var walkPos = _interactable.GetCurrentObjTargetPos();
        var lookPos = _interactable.GetCurrentObjPos();
        var outcome = data.Outcome;
        UnityAction callback = null;
        var player = OverworldManager.i.Player;

        if (outcome == TileInteractableOutcome.FIGHT) callback = () => StartFightFromInteractable(data.CombatOptions.items);
        if (outcome == TileInteractableOutcome.EVENT) callback = () => StartEventFromInteractable(data.EventOptions.items);
        if (outcome == TileInteractableOutcome.SHOP) callback = () => OpenShopFromInteractable(data.ShopData);
        if (_isWin) callback = () => GameManager.i.EndGame();

        player.MoveToTargetWithCallback(walkPos, lookPos, callback);
    }

    private void StartFightFromInteractable(List<CombatData> combatOptions)
    {
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
        OverworldManager.i.LoadCardGame(combatOptions[Random.Range(0, combatOptions.Count)]);
    }

    private void OpenShopFromInteractable(ShopData data)
    {
        UpdateEntranceVisuals();
        //_interactable.gameObject.SetActive(!_isUnlocked);
        OverworldUIManager.i.OpenShop(data);
    }

    private void StartEventFromInteractable(List<SpecialEventData> options)
    {
        UpdateEntranceVisuals();
        _interactable.gameObject.SetActive(!_isUnlocked);
        
        SpecialEventData chosenEvent = options[Random.Range(0, options.Count)];
        while (!chosenEvent.IsValidEvent()) {
            chosenEvent = options[Random.Range(0, options.Count)];
        }
        OverworldUIManager.i.StartEvent(chosenEvent);
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

    public int GetDifficulty() {return _difficulty;}
}