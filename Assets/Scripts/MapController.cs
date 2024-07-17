using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject _gridTilePrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private SelectableItem _mapSelectable;
    [SerializeField] private TileGenerator _tileGenerator;
    [SerializeField] private Transform _playerTile;

    [SerializeField] private Vector2 _mapScaleLimits = new Vector2(0.6f, 2.5f);
    [SerializeField] private float _mapZoomSpeed = 0.1f;

    private bool _panningMap;
    private Vector3 _oldMousePos;

    [SerializeField, ReadOnly] private Image[,] _imageGrid;
    private Vector2Int _dimensions => _tileGenerator.Dimensions;
    private void Update()
    {
        ZoomMap();
        PanMap();
    }

    public void Initialize()
    {
        GenerateBlankMap();
        gameObject.SetActive(false);
    }

    private void PanMap()
    {
        if (!_mapSelectable.Hovered) return;
        var newPos = Input.mousePosition;

        if ( !_panningMap) {
            if (!Input.GetMouseButtonDown(0)) return;
            
            _panningMap = true;
            _oldMousePos = newPos;
        }
        else if (Input.GetMouseButtonUp(0)) {
            _panningMap = false;
            return;
        }

        var mouseDelta = newPos - _oldMousePos;
        _gridParent.position += mouseDelta;
        _oldMousePos = newPos;
    }

    private void ZoomMap()
    {
        var delta = Input.mouseScrollDelta.y;
        delta = Mathf.Clamp(delta, -1, 1);
        var current = _gridParent.localScale.x;
        current += delta * _mapZoomSpeed;
        current = Mathf.Clamp(current, _mapScaleLimits.x, _mapScaleLimits.y);
        _gridParent.transform.localScale = Vector3.one * current;
    }

    public void CloseMap()
    {
        gameObject.SetActive(false);
    }

    public void OpenMap()
    {
        gameObject.SetActive(true);
        ResetMapView();
    }

    private void ResetMapView()
    {
        //_gridParent.localScale = Vector3.one;
        _gridParent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    private void GenerateBlankMap()
    {
        _imageGrid = new Image[_dimensions.x + 1, _dimensions.y + 1];
        for (int x = 0; x < _dimensions.x; x++) {
            for (int y = _dimensions.y - 1; y >= 0; y--) {
                var newTile = Instantiate(_gridTilePrefab, _gridParent);
                newTile.transform.SetSiblingIndex(1);
                newTile.name = x + ", " + y;
                _imageGrid[x,y] = newTile.GetComponentInChildren<Image>();
            }
        }
    }

    public void UpdatePlayerPosition(Vector2Int ID)
    {
        var currentTile = _imageGrid[ID.x, ID.y].transform;
        _playerTile.SetParent(currentTile);
        _playerTile.transform.localPosition = Vector3.zero;
    }

    public void revealRandomTiles(int numTiles)
    {
        var IDs = new List<Vector2Int>();
        while (IDs.Count < numTiles) {
            var option = new Vector2Int(Random.Range(0, _dimensions.x), Random.Range(0, _dimensions.y));
            if (!IDs.Contains(option)) IDs.Add(option);
        }
        foreach (var ID in IDs) RevealTile(ID);
    }

    public void RevealTile(Vector2Int ID)
    {
        _tileGenerator.GetComponent<TileGridController>().GetTile(ID).ShowOnMap();
    }

    public void RevealTile(Vector2Int ID, Sprite sprite, int turns)
    {
        if (_imageGrid[ID.x, ID.y].sprite == sprite) return;

        //print("updating tile: " + ID + ". sprite: " + sprite.name);
        _imageGrid[ID.x, ID.y].sprite = sprite;
        Rotate(_imageGrid[ID.x, ID.y].transform, turns);
    }

    private void Rotate(Transform obj, int turns)
    {
        if (turns == 2) turns = 0;
        else if (turns == 0) turns = 2;

        var z = 90 * turns;
        obj.localEulerAngles = new Vector3(0, 0, z);
        obj.gameObject.name += "(turns: " + turns + ". z: " + z + ")";
    }
}
