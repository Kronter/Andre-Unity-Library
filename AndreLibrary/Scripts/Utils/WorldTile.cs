using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile
{
    public Vector3Int LocalPlace { get; set; }

    public Vector3 WorldLocation { get; set; }

    public TileBase TileBase { get; set; }

    public UnityEngine.Tilemaps.Tile Tile{ get; set; }

    public Tilemap TilemapMember { get; set; }

    public Sprite OriginalSprite { get; set; }

    public string Name { get; set; }
}