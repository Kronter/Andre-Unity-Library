using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestOurTile : MonoBehaviour
{
    private WorldTile _tile;
    public Sprite toChange;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);

            var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

            if (tiles.TryGetValue(worldPoint, out _tile))
            {
                UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
                tile.sprite = toChange;
                _tile.TilemapMember.SetTile(_tile.LocalPlace, tile);
                _tile.TileBase = _tile.TilemapMember.GetTile(_tile.LocalPlace);
            }
        }
    }
}