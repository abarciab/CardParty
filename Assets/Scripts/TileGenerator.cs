using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(TileGridController))]
public class TileGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector2 _gridDimenstions = new Vector2(20, 20);
    [SerializeField] private float _tileWidth = 20;

    [SerializeField, ReadOnly] private List<TileController> _tiles = new List<TileController>();

    private TileGridController _gridController;

    private void Start()
    {
        _gridController = GetComponent<TileGridController>();
        GenerateGrid();
    }

    [ButtonMethod]
    private void GenerateGrid()
    {
        ClearGrid();
        for (int x = 0; x < _gridDimenstions.x; x++) {
            for (int y = 0; y < _gridDimenstions.y; y++) {
                PlaceTile(x, y);
            }
        }
        _gridController.SetTiles(_tiles);
    }

    private void ClearGrid()
    {
        foreach (var t in _tiles) Destroy(t.gameObject);
        _tiles.Clear();
    }

    private void PlaceTile(int x, int y)
    {
        var pos = transform.TransformPoint(x * _tileWidth, 0, y * _tileWidth);
        var halfDist = new Vector3(_gridDimenstions.x - 1, 0, _gridDimenstions.y - 1) * (_tileWidth / 2);
        pos -= halfDist;

        var newTileObj = Instantiate(_prefab, pos, Quaternion.identity, transform);
        var newTile = newTileObj.GetComponent<TileController>();
        bool _isMiddleTile = (x == Mathf.Floor(_gridDimenstions.x / 2)) && (y == Mathf.Floor(_gridDimenstions.y / 2));
        newTile.Initialize(x, y, _isMiddleTile, _gridController);
        _tiles.Add(newTile);
    }
}
