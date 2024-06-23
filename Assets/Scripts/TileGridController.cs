using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TileGridController : MonoBehaviour
{
    private List<TileController> _tiles = new List<TileController>();
    [SerializeField] private Transform _fogOfWar;

    public void SetTiles(List<TileController> tiles) => _tiles = tiles;

    public TileController GetTileInDirection(Vector2Int ID, Direction dir)
    {
        Vector2Int TargetID = new Vector2Int(ID.x, ID.y);
        if (dir == Direction.Up) TargetID.y += 1;
        if (dir == Direction.Right) TargetID.x += 1;
        if (dir == Direction.Down) TargetID.y -= 1;
        if (dir == Direction.Left) TargetID.x -= 1;

        var targetTile = _tiles.Where(x => x.GridPos == TargetID).ToList();
        if (targetTile.Count > 0) return targetTile[0];
        else return null;
    }

    public async void UpdateAllTiles(TileController newPlayerTile)
    {
        foreach (var t in _tiles) t.UpdateEntranceVisuals();
        await Task.Delay(500);
        _fogOfWar.transform.position = newPlayerTile.transform.position;
    }
}
