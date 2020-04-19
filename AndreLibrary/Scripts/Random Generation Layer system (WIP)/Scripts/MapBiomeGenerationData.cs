using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BiomeLayer
{
    [HideInInspector]
    public RandomGenerationLayer layer;
    public bool ShowLayer;
}

[CreateAssetMenu(fileName = "Map Biome Data", menuName = "Map Biome Data")]
public class MapBiomeGenerationData : ScriptableObject
{
    public List<RandomGenerationLayer> generationLayers;
    [Header("Do Not Change")]
    public List<BiomeLayer> OriginalBiomeLayer;
    [HideInInspector]
    public Vector2Int mapSize;

    public void Execute()
    {
        OriginalBiomeLayer = new List<BiomeLayer>();
        List<RandomGenerationLayer> OriginalLayer = new List<RandomGenerationLayer>();
        foreach (var layer in generationLayers)
        {
            layer.mapSize = mapSize;
            BiomeLayer tmp = new BiomeLayer();
            tmp.ShowLayer = true;
            tmp.layer = Instantiate(layer);
            OriginalBiomeLayer.Add(tmp);
        }

        foreach (var layer in OriginalBiomeLayer)
        {
            layer.layer.previousLayerData = OriginalLayer;
            layer.layer.Execute();
            OriginalLayer.Add(layer.layer);
        }
    }
}
