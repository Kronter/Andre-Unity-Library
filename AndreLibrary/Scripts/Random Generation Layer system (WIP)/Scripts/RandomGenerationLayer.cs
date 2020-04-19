using System.Collections.Generic;
using UnityEngine;

public enum LayerType
{
    Path,
    River,
    Forest,
    Mountain,
    Traversal
}

public class RandomGenerationLayer : ScriptableObject
{
    [HideInInspector]
    public Vector2Int mapSize;
    [HideInInspector]
    public bool[] layerData;
    [HideInInspector]
    public bool debug = false;
    [HideInInspector]
    public List<RandomGenerationLayer> previousLayerData;
    public LayerType layerTypeID;
    public List<int> entranceSides;
    public bool randomEntranceLocations;

    public void Execute()
    {
        LayerCreation();
    }

    protected virtual void LayerCreation()
    {

    }


    protected void OnDrawGizmos()
    {
        if (debug)
        {
            string mapLine = "";
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                    Color temp = Color.green;
                    if (layerData[index] != null)
                    {
                        temp = layerData[index] == true ? Color.red : Color.gray;
                    }
                    Gizmos.color = temp;
                    Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one);
                }
            }
        }
    }

    public List<bool[]>GetRegions(LayerType _layerTypeCheck, bool _tileType)
    {
        bool[] tmpRelevantLayerRegionData = new bool[mapSize.x * mapSize.y];
        int index = 0;
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                foreach (var layer in previousLayerData)
                {
                    if (layer.layerTypeID == _layerTypeCheck)
                    {
                        if(layer.layerData[index])
                        tmpRelevantLayerRegionData[index] = true;
                    }
                }
            }
        }

        List<List<Vector2Int>> regions = RegionFinder.DetectRegions(mapSize, tmpRelevantLayerRegionData, _tileType);
        List<bool[]> tmpRegions = new List<bool[]>();
        foreach (var region in regions)
        {
            bool[] tmpLayerData = new bool[mapSize.x * mapSize.y];
            foreach (var tile in region)
            {
                index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(tile.x, tile.y, mapSize.x);
                tmpLayerData[index] = true;
            }
            tmpRegions.Add(tmpLayerData);
        }
        return tmpRegions;
    }

    protected void SmoothMap(bool empty)
    {
        List<Vector2Int> tmp = new List<Vector2Int>();
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                int neighbourSimilarTiles = GetSurroundingSimilarCount(x, y,true);
                if (layerData[index] == true)
                {
                    if (neighbourSimilarTiles >= 2)
                        layerData[index] = true;
                    else if (neighbourSimilarTiles < 2)
                        layerData[index] = false;
                }
                else if (neighbourSimilarTiles >= 2)
                {
                    tmp.Add(new Vector2Int(x, y));
                }
            }
        }
        if (empty)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                    int neighbourSimilarTiles = GetSurroundingSimilarCount(x, y,false);
                    if (tmp.Contains(new Vector2Int(x, y)))
                    {
                        if (neighbourSimilarTiles >= 2)
                            layerData[index] = true;
                        else if (neighbourSimilarTiles < 2)
                            layerData[index] = false;
                    }
                }
            }
        }
    }

    protected int GetSurroundingSimilarCount(int gridX, int gridY, bool edge)
    {
        int similarCount = 0;
        for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
        {
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                if (neighbourY == gridY || neighbourX == gridX)
                {
                    if (neighbourX >= 0 && neighbourX < mapSize.x && neighbourY >= 0 && neighbourY < mapSize.y)
                    {
                        int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(neighbourX, neighbourY, mapSize.x);
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            similarCount += layerData[index] == true ? 1 : 0;
                        }
                    }
                    else if (edge)
                        similarCount++;
                }
            }
        }
        return similarCount;
    }

    protected void DrawCircle(Vector2Int _coord, int _brushSize)
    {
        for (int y = 0; y <= _brushSize; y++)
        {
            for (int x = 0; x <= _brushSize; x++)
            {
                if (x * x + y * y <= _brushSize * _brushSize)
                {
                    int drawX = _coord.x + x;
                    int drawY = _coord.y + y;
                    if (drawX >= 0 && drawX < mapSize.x && drawY >= 0 && drawY < mapSize.y)
                    {
                        int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(drawX, drawY, mapSize.x);
                        layerData[index] = true;
                    }
                }
            }
        }
    }
}
