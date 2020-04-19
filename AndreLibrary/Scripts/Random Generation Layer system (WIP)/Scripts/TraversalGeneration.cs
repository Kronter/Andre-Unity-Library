using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Traversal Layer", menuName = "Random Generation Layer/Traversal Layer")]
public class TraversalGeneration : RandomGenerationLayer
{
    public LayerType layerTypeCheck;
    [HideInInspector]
    public bool[] relevantLayerRegionData;
    // Start is called before the first frame update
    protected override void LayerCreation()
    {
        layerData = new bool[mapSize.x * mapSize.y];
        List<bool[]> regions = GetRegions(layerTypeCheck, false);
        int index = 0;
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                foreach (var region in regions)
                {
                    index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                }
            }
        }
    }
}
