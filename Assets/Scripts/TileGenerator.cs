using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector2 _gridDimenstions = new Vector2(20, 20);
    [SerializeField] private float _tileWidth = 20;
    [SerializeField, ReadOnly] private List<GameObject> _tiles = new List<GameObject>();

    private void Start()
    {
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
    }

    private void ClearGrid()
    {
        foreach (var t in _tiles) Destroy(t);
        _tiles.Clear();
    }

    private void PlaceTile(int x, int y)
    {
        var pos = transform.TransformPoint(x * _tileWidth, 0, y * _tileWidth);
        var halfDist = new Vector3(_gridDimenstions.x - 1, 0, _gridDimenstions.y - 1) * (_tileWidth / 2);
        pos -= halfDist;

        var newTile = Instantiate(_prefab, pos, Quaternion.identity, transform);
        _tiles.Add(newTile);
    }
}
