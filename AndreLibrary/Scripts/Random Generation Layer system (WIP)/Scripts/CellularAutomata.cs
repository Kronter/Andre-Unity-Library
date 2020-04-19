using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CellularAutomataSmoothingLayer
{
    public bool encourageMapEdges;
    public int ChangeTreshold;
    public bool upperTresholdEqualsTo;
    public bool lowerTresholdEqualsTo;
}

public static class CellularAutomata 
{
    private static Vector2Int mapSize;
    private static int randomFillPercent;
    private static bool[] mapData;
    private static string seed;
    private static Vector2Int mapBuffer;
    private static List<CellularAutomataSmoothingLayer> SmoothingLayers;
    private static bool coverEdges = true;


    public static bool[] GeneratedMap(Vector2Int _mapSize, int _randomFillPercent, Vector2Int _mapBuffer, List<CellularAutomataSmoothingLayer> _SmoothingLayers, bool _coverEdges = false, string _seed = "")
    {
        mapSize = _mapSize;
        randomFillPercent = _randomFillPercent;
        mapBuffer = _mapBuffer;
        SmoothingLayers = _SmoothingLayers;
        coverEdges = _coverEdges;
        if (_seed == "")
        {
            seed = Time.time.ToString();
        }
        else
        {
            seed = _seed;
        }
        mapData = new bool[mapSize.x * mapSize.y];
        RandomFillMap();
        if (coverEdges)
            CoverSides();
        SmoothMap();
        return mapData;
    }

    private static void RandomFillMap()
    {
        System.Random pseudoRand = new System.Random(seed.GetHashCode());

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                if(x < mapBuffer.x || y < mapBuffer.y || x >= mapSize.x - mapBuffer.x || y >= mapSize.y - mapBuffer.y)
                    mapData[index] = false;
                else
                    mapData[index] = (pseudoRand.Next(0, 100) < randomFillPercent) ? true : false;
            }
        }
    }

    private static void CoverSides()
    {
        System.Random pseudoRand = new System.Random(seed.GetHashCode());

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                if (x < mapBuffer.x || y < mapBuffer.y || x >= mapSize.x - mapBuffer.x || y >= mapSize.y - mapBuffer.y)
                    mapData[index] = true;
            }
        }
    }

    private static void SmoothMap()
    {
        for (int i = 0; i < SmoothingLayers.Count; i++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    int neighbourSimilarTiles = GetSurroundingSimilarCount(x, y, i);
                    int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                    if (SmoothingLayers[i].upperTresholdEqualsTo)
                    {
                        if (neighbourSimilarTiles >= SmoothingLayers[i].ChangeTreshold)
                            mapData[index] = true;
                    }
                    else
                    {
                        if (neighbourSimilarTiles > SmoothingLayers[i].ChangeTreshold)
                            mapData[index] = true;
                    }
                    if (SmoothingLayers[i].lowerTresholdEqualsTo)
                    {
                        if (neighbourSimilarTiles <= SmoothingLayers[i].ChangeTreshold)
                            mapData[index] = false;
                    }
                    else
                    {
                        if (neighbourSimilarTiles < SmoothingLayers[i].ChangeTreshold)
                            mapData[index] = false;
                    }
                }
            }
        }
    }

    private static int GetSurroundingSimilarCount(int gridX,int gridY, int smoothingLayersIndex)
    {
        int similarCount = 0;
        for (int neighbourY = gridY -1; neighbourY <= gridY+1; neighbourY++)
        {
            for (int neighbourX = gridX-1; neighbourX <= gridX+1; neighbourX++)
            {
                if (neighbourX >= 0 && neighbourX < mapSize.x && neighbourY >= 0 && neighbourY < mapSize.y)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(neighbourX, neighbourY, mapSize.x);
                        similarCount += mapData[index] == true ? 1 : 0;
                    }
                }
                else if (SmoothingLayers[smoothingLayersIndex].encourageMapEdges)
                        similarCount++;
            }
        }
        return similarCount;
    }
}
