using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TileGridController : MonoBehaviour
{
    private TileController[,] _tiles;
    [SerializeField] private Transform _fogOfWar;

    public void SetTiles(TileController[,] tiles) => _tiles = tiles;

    [ButtonMethod]
    private void RevealEntireMap()
    {
        foreach (var t in _tiles) t.ShowOnMap();
    }

    public TileController GetTile(Vector2Int ID)
    {
        return _tiles[ID.x, ID.y];
    }

    public TileController GetTileInDirection(Vector2Int ID, Direction dir)
    {
        Vector2Int targetID = new Vector2Int(ID.x, ID.y);
        if (dir == Direction.UP) targetID.y += 1;
        if (dir == Direction.RIGHT) targetID.x += 1;
        if (dir == Direction.DOWN) targetID.y -= 1;
        if (dir == Direction.LEFT) targetID.x -= 1;

        if (targetID.x >= _tiles.GetLength(0) || targetID.y >= _tiles.GetLength(1) || targetID.x == 0 || targetID.y == 0) return null;
        return _tiles[targetID.x, targetID.y];
    }

    public async void UpdateAllTiles(TileController newPlayerTile)
    {
        foreach (var t in _tiles) if (t) t.UpdateEntranceVisuals();
        await Task.Delay(500);
        _fogOfWar.transform.position = newPlayerTile.transform.position;
    }
}
