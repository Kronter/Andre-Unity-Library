using System.Collections.Generic;
using UnityEngine;

public class LayerBasedGeneration : MonoBehaviour
{
    public Vector2Int mapSize;
    public MapBiomeGenerationData biomeGeneratorData;
    public bool debugMap = false;
    public bool debugMapRegion = false;
    public LayerType layerRegionTypeCheck;
    public bool layerRegionReverseCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        biomeGeneratorData.mapSize = mapSize;
        //while (true)
        //{
            biomeGeneratorData.Execute();
        //    if (MapPassCheck())
        //        break;
        //}

        //if (debugMap)
        //{
        //    string mapLine = "";
        //    for (int y = 0; y < mapSize.y; y++)
        //    {
        //        for (int x = 0; x < mapSize.x; x++)
        //        {
        //            int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
        //            string temp = " ";
        //            foreach (var layer in biomeGeneratorData.generationLayers)
        //            {
        //                if (layer.layerTypeID == 0)
        //                    temp = layer.layerData[index] == true ? "P " : ".  ";
        //                else if (layer.layerTypeID == 1 && temp == ".  ")
        //                    temp = layer.layerData[index] == true ? "R " : ".  ";
        //            }
        //            mapLine += temp;
        //        }
        //        mapLine += "\n";
        //    }
        //    Debug.Log(mapLine);
        //}
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            biomeGeneratorData.mapSize = mapSize;
            //while (true)
            //{
                biomeGeneratorData.Execute();
            //    if (MapPassCheck())
            //        break;
            //}

        }
    }

    private bool MapPassCheck()
    {
        int numberOfRiverLayers = 0;
        int numberOfRiverConnections = 0;
        List<RandomGenerationLayer> riverConnectionCheckLayers = new List<RandomGenerationLayer>();
        foreach (var Biomelayer in biomeGeneratorData.OriginalBiomeLayer)
        {
            if (Biomelayer.layer.layerTypeID == LayerType.Path)
                riverConnectionCheckLayers.Add(Biomelayer.layer);
            else if (Biomelayer.layer.layerTypeID == LayerType.River)
            {
                riverConnectionCheckLayers.Add(Biomelayer.layer);
                numberOfRiverLayers++;
            }
        }

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                Color temp = Color.white;
                foreach (var layer in riverConnectionCheckLayers)
                {
                    if (layer != null)
                    {
                        if (layer.layerTypeID == LayerType.Path)
                            temp = layer.layerData[index] == true ? Color.gray : temp;
                        else if (layer.layerTypeID == LayerType.River && temp == Color.white)
                            temp = layer.layerData[index] == true ? Color.blue : temp;
                        else if (layer.layerTypeID == LayerType.River && temp == Color.gray)
                            temp = layer.layerData[index] == true ? Color.yellow : temp;

                        if (temp == Color.yellow)
                            numberOfRiverConnections++;
                    }
                }
            }
        }

        return numberOfRiverLayers <= numberOfRiverConnections;
    }

    private void OnDrawGizmos()
    {
        if (debugMapRegion)
        {
            if (biomeGeneratorData.OriginalBiomeLayer == null)
                return;
            List<bool[]> regions = biomeGeneratorData.OriginalBiomeLayer[biomeGeneratorData.OriginalBiomeLayer.Count - 1].layer.GetRegions(layerRegionTypeCheck, layerRegionReverseCheck);
            int index = 0;
            float colour = 255f;
            Color temp = Color.white;
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    Gizmos.color = temp;
                    Gizmos.DrawCube(new Vector3(x * 10, -1f, y * 10), new Vector3(10, 0.01f, 10));
                }
            }

            foreach (var region in regions)
            {
                temp = Color.white;
                for (int y = 0; y < mapSize.y; y++)
                {
                    for (int x = 0; x < mapSize.x; x++)
                    {
                        index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                        if (region[index])
                        {
                            temp = new Color(0, 0.5f, colour / 255f, 1);
                            Gizmos.color = temp;
                            Gizmos.DrawCube(new Vector3(x * 10, -1f, y * 10), new Vector3(10, 0.01f, 10));
                        }
                    }
                }
                colour = colour - (colour/regions.Count);
            }
        }
        else if (debugMap)
        {
            if (biomeGeneratorData.OriginalBiomeLayer == null)
                return;

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
                    Color temp = Color.white;
                    foreach (var Biomelayer in biomeGeneratorData.OriginalBiomeLayer)
                    {
                        if (Biomelayer.layer != null)
                        {
                            if (!Biomelayer.ShowLayer)
                                continue;
                            if (Biomelayer.layer.layerTypeID == LayerType.Path)
                                temp = Biomelayer.layer.layerData[index] == true ? Color.gray : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.River && temp == Color.white)
                                temp = Biomelayer.layer.layerData[index] == true ? Color.blue : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.River && temp == Color.gray)
                                temp = Biomelayer.layer.layerData[index] == true ? Color.yellow : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Forest && temp != Color.gray && temp != Color.blue && temp != Color.yellow)
                                temp = Biomelayer.layer.layerData[index] == true ? Color.green : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Mountain && temp == Color.green)
                                temp = Biomelayer.layer.layerData[index] == true ? new Color(64.0f / 255f, 79.0f / 255f, 36.0f / 255f) : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Mountain && temp == Color.blue)
                                temp = Biomelayer.layer.layerData[index] == true ? Color.cyan : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Mountain && temp == Color.gray)
                                temp = Biomelayer.layer.layerData[index] == true ? new Color(190.0f / 255f, 85.0f / 255f, 4.0f / 255f) : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Mountain && temp == Color.yellow)
                                temp = Biomelayer.layer.layerData[index] == true ? new Color(188.0f / 255f, 158.0f / 255f, 130.0f / 255f) : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Mountain)
                                temp = Biomelayer.layer.layerData[index] == true ? new Color(92.0f / 255f, 51.0f / 255f, 23.0f / 255f) : temp;
                            else if (Biomelayer.layer.layerTypeID == LayerType.Traversal)
                                temp = Biomelayer.layer.layerData[index] == true ? Color.red : temp;
                        }
                    }
                    Gizmos.color = temp;
                    Gizmos.DrawCube(new Vector3(x * 10, -1f, y * 10), new Vector3(10, 0.01f, 10));
                }
            }
        }
    }

}
