using System.Collections.Generic;
using UnityEngine;

public static class RegionFinder
{
    public static List<List<Vector2Int>> DetectRegions(Vector2Int _mapSize, bool[] _map, bool _tileType)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        bool[] mapFlags = new bool[_mapSize.x * _mapSize.y];
        int index = 0; 
        for (int y = 0; y < _mapSize.y; y++)
        {
            for (int x = 0; x < _mapSize.x; x++)
            {
                index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, _mapSize.x);
                if (mapFlags[index] == false && _map[index] == _tileType)
                {
                    List<Vector2Int> newRegion = GetRegionTiles(_mapSize, _map, _tileType, x, y);
                    regions.Add(newRegion);
                    foreach (var tile in newRegion)
                    {
                        index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(tile.x, tile.y, _mapSize.x);
                        mapFlags[index] = true;
                    }
                }
            }
        }

        return regions;
    }

    // flood fill
    public static List<Vector2Int> GetRegionTiles(Vector2Int _mapSize, bool[] _map, bool _tileType, int _startX, int _startY)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        bool[] mapFlags = new bool[_mapSize.x * _mapSize.y];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(_startX, _startY));
        int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(_startX, _startY, _mapSize.x);
        mapFlags[index] = true;

        while(queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);

            for (int y = tile.y - 1; y <= tile.y + 1; y++)
            {
                for (int x = tile.x - 1; x <= tile.x + 1; x++)
                {
                    if ((x >= 0 && x < _mapSize.x && y >= 0 && y < _mapSize.y) && (y == tile.y || x == tile.x))
                    {
                        index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, _mapSize.x);
                        if (mapFlags[index] == false && _map[index] == _tileType)
                        {
                            mapFlags[index] = true;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        return tiles;
    }
}