using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Forest Layer", menuName = "Random Generation Layer/Forest Layer")]
public class ForestGeneration : RandomGenerationLayer
{
    [Range(0,100)]
    public int randomFillPercentage = 50;
    public string seed;
    public bool useRandomSeed;
    public Vector2Int mapBufferMax;
    public Vector2Int mapBufferMin;
    public List<CellularAutomataSmoothingLayer> SmoothingLayers;
    // Start is called before the first frame update
    protected override void LayerCreation()
    {
        if (SmoothingLayers == null)
        {
            SmoothingLayers = new List<CellularAutomataSmoothingLayer>();
            CellularAutomataSmoothingLayer layer = new CellularAutomataSmoothingLayer();
            layer.ChangeTreshold = 4;
            layer.encourageMapEdges = false;
            layer.lowerTresholdEqualsTo = false;
            layer.upperTresholdEqualsTo = false;
            SmoothingLayers.Add(layer);
        }

        Vector2Int buffer = new Vector2Int(Random.Range(mapBufferMin.x, mapBufferMax.x + 1), Random.Range(mapBufferMin.y, mapBufferMax.y + 1));
        if (useRandomSeed)
            layerData = CellularAutomata.GeneratedMap(mapSize, randomFillPercentage, buffer, SmoothingLayers);
        else
            layerData = CellularAutomata.GeneratedMap(mapSize, randomFillPercentage, buffer, SmoothingLayers, false,seed);
    }
}
