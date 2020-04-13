using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour
{
    public static GameTiles instance;
    public Tilemap Tilemap;

    public Dictionary<Vector3, WorldTile> tiles;

    private void Awake()
    {
        GetWorldTiles();
    }

    // Use this for initialization
    private void GetWorldTiles()
    {
        tiles = new Dictionary<Vector3, WorldTile>();
        foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
        {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

            if (!Tilemap.HasTile(localPlace)) continue;
            var tile = new WorldTile
            {
                LocalPlace = localPlace,
                WorldLocation = Tilemap.CellToWorld(localPlace),
                TileBase = Tilemap.GetTile(localPlace),
                TilemapMember = Tilemap,
                Name = localPlace.x + "," + localPlace.y,
                Tile = Tilemap.GetTile<UnityEngine.Tilemaps.Tile>(localPlace),
                OriginalSprite = Tilemap.GetTile<UnityEngine.Tilemaps.Tile>(localPlace).sprite
            };

            tiles.Add(tile.WorldLocation, tile);
        }
    }

    public void resetTiles()
    {
        WorldTile _tile;
        foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
        {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
           if(tiles.TryGetValue(Tilemap.CellToWorld(localPlace), out _tile))
            {
                _tile.Tile.sprite = _tile.OriginalSprite;
                _tile.TilemapMember.RefreshTile(_tile.LocalPlace);
            }
        }
    }
}