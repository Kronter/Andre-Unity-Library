using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andre.Utils.RandomGeneration;
using System.IO;
using Andre.AutoTiling;

public class TestDrunkWalkGen : MonoBehaviour
{
    [SceneNamePicker(showPath = false)]
    public string SceneName;
    public GameObject obj;
    public TileAsset asset;
    public bool Generate = false;
    public int mapHeight = 25;
    public int mapWidth = 25;
    public int NumberOfDrunks = 5;
    public float DrunkSteps = 400;
    public bool wallOrGround = true;
    public bool fourBit = true;
    public bool Borders = true;
    Color CheckColour = Color.black;
    GameObject parent;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        CheckColour = wallOrGround ? Color.black : Color.white;
        DrunkenWalkGen.height = mapHeight;
        DrunkenWalkGen.width = mapWidth;
        DrunkenWalkGen.NumberOfDrunks = NumberOfDrunks;
        if(Borders)
        DrunkenWalkGen.Padding = new Vector2(1, 1);
        else
            DrunkenWalkGen.Padding = new Vector2(-1, -1);
        DrunkenWalkGen.DrunkSteps = DrunkSteps;
        DrunkenWalkGen.wallColour = Color.black;
        DrunkenWalkGen.groundColour = Color.white;
        DrunkenWalkGen.drunkExit = new Vector3(0, 100, 0);
        Texture2D tex = DrunkenWalkGen.DrunkWalkGenMap(Vector3.zero);

        parent = new GameObject();
        parent.transform.position = this.transform.position;
        Tile(tex);

        byte[] bytes = tex.EncodeToPNG();
        var dirPath = Application.dataPath + "/AndreLibrary/Examples/Auto Tiling/SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
        //Debug.Log(dirPath);
    }

    private void Update()
    {
        if (Generate)
        {
            Destroy(parent);
            Init();
            Generate = false;
        }
    }

    void Tile(Texture2D tex)
    {
        Vector3 tmp = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        for (int x = 0; x < 25; x++)
        {
            for (int y = 0; y < 25; y++)
            {
                if (fourBit)
                {
                    if (tex.GetPixel(x, y) == CheckColour)
                    {
                        GameObject sprite = Instantiate(obj, tmp, Quaternion.identity, parent.transform);
                        int index = (int)Andre.Utils.AutoTiling.CalculateTileFlags4Bit(
                                       tex.CheckIsColour(x, y, Vector2Int.right, Color.black),
                                       tex.CheckIsColour(x, y, Vector2Int.left, Color.black),
                                       tex.CheckIsColour(x, y, Vector2Int.up, Color.black),
                                       tex.CheckIsColour(x, y, Vector2Int.down, Color.black));
                        sprite.GetComponent<SpriteRenderer>().sprite = asset.tiles[index];
                    }
                }
                else
                {
                    if (tex.GetPixel(x, y) == CheckColour)
                    {
                        GameObject sprite = Instantiate(obj, tmp, Quaternion.identity, parent.transform);
                        int index = Andre.Utils.AutoTiling.TileEightBitStandardConversion(Andre.Utils.AutoTiling.CalculateTileFlags8Bit(
                                       tex.CheckIsColour(x, y, Vector2Int.right, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.left, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.up, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.down, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.left + Vector2Int.up, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.right + Vector2Int.up, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.left + Vector2Int.down, CheckColour),
                                       tex.CheckIsColour(x, y, Vector2Int.right + Vector2Int.down, CheckColour)));
                        sprite.GetComponent<SpriteRenderer>().sprite = asset.tiles[index];
                    }
                }
                tmp.y += 0.638f;
            }
            tmp.y = 0;
            tmp.x += 0.638f;
        }
    }
}
