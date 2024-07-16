using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(TileGridController))]
public class TileGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabTiles = new List<GameObject>();
    [SerializeField] private Vector2 _gridDimenstions = new Vector2(20, 20);
    [SerializeField] private float _tileWidth = 20;

    [SerializeField, ReadOnly] private List<TileController> _placedTiles = new List<TileController>();

    [Header("Probabilities")]
    [SerializeField, Range(0, 1)] private float _clampPathChance = 0.75f; 
    [SerializeField, Range(0, 1)] private float _largePatchChance = 0.5f;

    [Header("Interactables")]
    [SerializeField] private List<TileInteractableData> _tileInteractableOptions = new List<TileInteractableData>();

    [Header("Special tiles")]
    [SerializeField] private GameObject _startTile;
    [SerializeField] private GameObject _winTile;
    [SerializeField] private float _winTileMinDist;

    private TileController[,] _tileGrid;

    private TileGridController _gridController;
    private int _numPaths;
    private int _targetPathNum = 4;
    private Transform _transform;
    private bool _placedWinTile;
    private bool _failed;
    private const int _maxTries = 10;
    private int _numFails;

    private Vector2Int _centerPos => new Vector2Int((int)_gridDimenstions.x / 2, (int)_gridDimenstions.y / 2);

    public Vector2Int Dimensions => new Vector2Int((int)_gridDimenstions.x, (int)_gridDimenstions.y);

    private void Start()
    {
        _transform = transform;
        _gridController = GetComponent<TileGridController>();
        RegenerateGrid();
    }

    [ButtonMethod]
    private void RegenerateGrid()
    {
        OverworldUIManager.i.Createmap();
        ClearGrid();
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        print("Generating grid");
        _tileGrid = new TileController[(int)_gridDimenstions.x + 1, (int)_gridDimenstions.y + 1];

        var current = new Vector2Int(_centerPos.x, _centerPos.y); 

        List<Vector2Int> directions = new List<Vector2Int>() { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };
        var dirIndex = 0;
        var steps = 1;
        var stepsCount = 0;
        var changeDirCount = 0;
        var total = _gridDimenstions.x * _gridDimenstions.y;

        for (int i = 0; i < total; i++) {
            PlaceTile(current.x, current.y, current == _centerPos);
            if (_failed) {
                print("failed, trying again...");
                _numFails += 1;
                RegenerateGrid();
                return;
            }

            current.x += directions[dirIndex].x;
            current.y += directions[dirIndex].y;
            stepsCount += 1;

            if (stepsCount == steps) {
                stepsCount = 0;
                dirIndex = (dirIndex + 1) % 4;
                changeDirCount += 1;
                if (changeDirCount % 2 == 0) steps += 1;
            }
        }
        _gridController.SetTiles(_placedTiles);

        if (_numFails > 0) print("Failed: " + _numFails + " times");
    }

    private void ClearGrid()
    {
        foreach (var t in _placedTiles) Destroy(t.gameObject);
        _placedTiles.Clear();
        _placedWinTile = false;
        _failed = false;
    }

    private void PlaceTile(int x, int y, bool isCenter)
    {
        print("placing tile");
        var pos = _transform.TransformPoint(x * _tileWidth, 0, y * _tileWidth);
        var halfDist = new Vector3(_gridDimenstions.x - 1, 0, _gridDimenstions.y - 1) * (_tileWidth / 2);
        pos -= halfDist;

        var prefabData = SelectPrefab(x, y, isCenter);
        if (prefabData.Item1 == null) {
            _failed = true;
            return;
        }
        var selectedInteractable = _tileInteractableOptions[Random.Range(0, _tileInteractableOptions.Count)]; 

        var newTileObj = Instantiate(prefabData.Item1, pos, prefabData.Item2, _transform);
        var newTile = newTileObj.GetComponent<TileController>();

        newTile.Initialize(x, y, isCenter, _gridController, prefabData.Item2, new TileInteractableData(selectedInteractable));

        _placedTiles.Add(newTile);
        _tileGrid[x,y] = newTile;
        CountPaths();
    }

    private void CountPaths()
    {
        int foundPaths = 0;
        foreach (var t in _placedTiles) {
            var pos = t.GridPos;
            var edges = GetEdgesOfHole(pos);
            if (GetTileAt(pos + Vector2Int.up) == null) foundPaths += CountPathsInEdge(pos, Direction.UP);
            if (GetTileAt(pos + Vector2Int.right) == null) foundPaths += CountPathsInEdge(pos, Direction.RIGHT);
            if (GetTileAt(pos + Vector2Int.down) == null) foundPaths += CountPathsInEdge(pos, Direction.DOWN);
            if (GetTileAt(pos + Vector2Int.left) == null) foundPaths += CountPathsInEdge(pos, Direction.LEFT);
        }
        _numPaths = foundPaths;
    }

    private int CountPathsIntoHole(List<string> hole)
    {
        int paths = 0;
        foreach (var edge in hole) paths += edge.Split("P").Length - 1;
        return paths;
    }

    private int CountPathsInEdge(Vector2Int pos, Direction dir)
    {
        var edge = GetEdgeAt(pos, dir);
        return edge.Split("P").Length - 1;
    }

    private (GameObject, Quaternion) SelectPrefab(int x, int y, bool center)
    {
        if (center) return new(_startTile, Quaternion.identity);

        var hole = GetEdgesOfHole(x, y);
        var validTiles = new List<(GameObject, Quaternion)>();
        var pos = new Vector2Int(x, y);

        AddAllPossibleValidTiles(ref validTiles, hole);
        if (Random.Range(0f, 1) < _clampPathChance) LimitPathNum(ref validTiles, pos);
        if (Random.Range(0f, 1) < _largePatchChance) LimitLargePatch(ref validTiles, pos, hole);

        if (validTiles.Count == 0) {
            Debug.Log("Couldn't find valid tile for hole: " + string.Join("|", hole));
            return new (null, Quaternion.identity);
        }
        var chosenTile = validTiles[Random.Range(0, validTiles.Count)];

        if (ShouldPlaceWinTile(x, y)) {
            _placedWinTile = true;
            chosenTile.Item1 = _winTile;
        }

        return chosenTile;
    }

    private bool ShouldPlaceWinTile(int x, int y) {
        if (_placedWinTile) return false;
        var distance = Vector2Int.Distance(new Vector2Int(x, y), _centerPos);
        if (distance < _winTileMinDist) return false;
        if (x == _gridDimenstions.x - 1 && y == _gridDimenstions.y - 1) return true;
        else return (Random.Range(0f, 1) < 0.1f);
    }

    private void AddAllPossibleValidTiles(ref List<(GameObject, Quaternion)> validTiles, List<string> hole)
    {
        foreach (var t in _prefabTiles) {
            var WFC = t.GetComponent<TileController>().WFCInfo;
            if (WFC.GetValidRotation(hole, out Quaternion rot)) validTiles.Add((t, rot));
        }
    }

    private void LimitLargePatch(ref List<(GameObject, Quaternion)> validTiles, Vector2Int pos, List<string> hole)
    {
        string holeTotalString = string.Join("", hole);
        var differentTypes = "";
        foreach (var character in holeTotalString) if (!differentTypes.Contains(character.ToString())) differentTypes += character;

        var indicesToRemove = new List<int>();
        for (int i = 0; i < validTiles.Count; i++) {
            var data = validTiles[i].Item1.GetComponent<TileController>().WFCInfo;
            if (data.HasOtherTypes(differentTypes) && validTiles.Count - indicesToRemove.Count > 0) indicesToRemove.Add(i);  
        }

        RemoveFromIndexList(ref validTiles, indicesToRemove);
    }

    private void LimitPathNum(ref List<(GameObject, Quaternion)> validTiles, Vector2Int pos)
    {
        CountPaths();

        var hole = GetEdgesOfHole(pos);
        int pathsIntoHole = CountPathsIntoHole(hole);
        List<int> emptyEdges = GetEmptyEdges(hole);

        if (_numPaths != _targetPathNum) CorrectForTooManyOrTooFewPaths(ref validTiles, emptyEdges, pathsIntoHole);
        if (pathsIntoHole == 0) LimitNewOrphanPaths(ref validTiles, emptyEdges);
    }

    private void CorrectForTooManyOrTooFewPaths(ref List<(GameObject, Quaternion)> validTiles, List<int> emptyEdges, int pathsIntoHole)
    {
        bool tooManyPaths = _numPaths > _targetPathNum;
        bool tooFewPaths = _numPaths < _targetPathNum;

        List<int> invalidIndices = new List<int>();
        for (int i = 0; i < validTiles.Count; i++) {
            int newPaths = GetNewPathsIfPlaced(validTiles[i], emptyEdges);
            int pathDelta = newPaths - pathsIntoHole;

            if (tooManyPaths && pathDelta > 0) invalidIndices.Add(i);
            else if (tooFewPaths && pathDelta < 0 && pathsIntoHole > 0) invalidIndices.Add(i);
        }

        RemoveFromIndexList(ref validTiles, invalidIndices);
    }

    private List<int> GetEmptyEdges(List<string> hole)
    {
        List<int> emptyEdges = new List<int>();
        for (int i = 0; i < hole.Count; i++) if (string.IsNullOrEmpty(hole[i])) emptyEdges.Add(i);
        return emptyEdges;
    }

    private void LimitNewOrphanPaths(ref List<(GameObject, Quaternion)> validTiles, List<int> emptyEdges)
    {
        var invalidIndices = new List<int>();
        int removed = 0;
        for (int i = 0; i < validTiles.Count; i++) {
            int newPaths = GetNewPathsIfPlaced(validTiles[i], emptyEdges);
            if (newPaths == 0 || validTiles.Count - removed <= 0) continue;
            
            invalidIndices.Add(i);
            removed++;   
        }

        RemoveFromIndexList(ref validTiles, invalidIndices);
    }

    private void RemoveFromIndexList(ref List<(GameObject, Quaternion)> validTiles, List<int> invalidIndices)
    {
        for (int i = invalidIndices.Count - 1; i >= 0; i--) validTiles.RemoveAt(invalidIndices[i]);
    }

    private int GetNewPathsIfPlaced((GameObject, Quaternion) tileData, List<int> emptyEdges)
    {
        var wfcInfo = tileData.Item1.GetComponent<TileController>().WFCInfo;
        var rot = tileData.Item2;

        var newEdges = wfcInfo.GetEdges(rot);
        int newPaths = 0;
        foreach (var edgeIndex in emptyEdges) newPaths += newEdges[edgeIndex].Split("P").Length - 1;
        return newPaths;
    }

    private List<string> GetEdgesOfHole(Vector2Int pos) => GetEdgesOfHole(pos.x, pos.y);

    private List<string> GetEdgesOfHole(int x, int y)
    {
        var edges = new List<string>();
        edges.Add(GetEdgeAt(x, y + 1, Direction.DOWN, true));
        edges.Add(GetEdgeAt(x + 1, y, Direction.LEFT, true));
        edges.Add(GetEdgeAt(x, y - 1, Direction.UP, true));
        edges.Add(GetEdgeAt(x - 1, y, Direction.RIGHT, true));
        return edges;
    }

    private TileController GetTileAt(Vector2Int pos) => GetTileAt(pos.x, pos.y);

    private TileController GetTileAt(int x, int y)
    {
        if (x > _gridDimenstions.x || y > _gridDimenstions.y || x < 0 || y < 0) return null;
        return _tileGrid[x, y];
    }

    private string GetEdgeAt(Vector2Int pos, Direction dir) => GetEdgeAt(pos.x, pos.y, dir);

    private string GetEdgeAt(int x, int y, Direction dir, bool inverse = false)
    {
        var tile = GetTileAt(x, y);
        if (!tile) return "";
        var edge = tile.WFCInfo.GetEdgeAt(dir);
        if (inverse) {
            var charArray = edge.ToCharArray();
            System.Array.Reverse(charArray);
            return new string(charArray);
        }
        else return edge;
    }
}
