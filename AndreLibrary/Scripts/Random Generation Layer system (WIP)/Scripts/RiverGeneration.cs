using UnityEngine;

[CreateAssetMenu(fileName = "River Layer", menuName = "Random Generation Layer/River Layer")]
public class RiverGeneration : RandomGenerationLayer
{
    public bool useFavourDirection = true;
    public bool smoothEmpty = true;
    public float changeDirectionOdds = 0.8f;
    public int riverSize = 1;
    public int numberOfRivers = 1;
    public bool randomSide = true;
    // Start is called before the first frame update
    protected override void LayerCreation()
    {
        layerData = new bool[mapSize.x * mapSize.y];
        int sides = 0;
        for (int i = 0; i < numberOfRivers; i++)
        {
            if (entranceSides.Count > 0)
            {
                RiverDrunkWalk(entranceSides[sides]);
                sides++;
                if (sides > entranceSides.Count - 1)
                    sides = 0;
            }
            else
            {
                randomEntranceLocations = true;
                RiverDrunkWalk(sides);
                sides++;
                if (sides > 3)
                    sides = 0;
            }
        }
        SmoothMap(smoothEmpty);
        //if (debug)
        //{
        //    string mapLine = "";
        //    for (int y = 0; y < mapSize.y; y++)
        //    {
        //        for (int x = 0; x < mapSize.x; x++)
        //        {
        //            int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(x, y, mapSize.x);
        //            mapLine += layerData[index] == true ? "R " : ".  "; 
        //        }
        //        mapLine += "\n";
        //    }
        //    Debug.Log(mapLine);
        //}
    }

    void RiverDrunkWalk(int side)
    {
        int startSide = 0;
        if (randomSide)
            startSide = Andre.Utils.MathUtils.RandomEqualChanceInt(4);
        else
            startSide = side;
        Vector2Int Initpos = randomEntranceLocations == true ? initializeRandomStartLocation(startSide) : initializeStartLocation(startSide);
        Vector2Int pos = Initpos;
        int index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(pos.x, pos.y, mapSize.x);
        layerData[index] = true;
        Vector2Int movingDirection = Vector2Int.up;
        Vector2Int favourDirection = Vector2Int.up;
        int direction = 0;
        int backDirection = 0;

        switch (startSide)
        {
            case 0:
                direction = 2;
                movingDirection = Vector2Int.down;
                break;
            case 1:
                direction = 3;
                movingDirection = Vector2Int.right;
                break;
            case 2:
                direction = 0;
                movingDirection = Vector2Int.up;
                break;
            case 3:
                direction = 1;
                movingDirection = Vector2Int.left;
                break;
            default:
                break;
        }
        favourDirection = movingDirection;
        bool toBuild = true;
        int endlessCheck = 0;
        while (toBuild)
        {
            float chance = Random.Range(0.0f, 1.0f);
            if (chance <= changeDirectionOdds)
            {
                int tmpDirection = direction;
                while (true)
                {
                    direction = Andre.Utils.MathUtils.RandomEqualChanceInt(useFavourDirection == true ?5 :4);
                    if (direction != startSide && direction != tmpDirection)
                        break;
                }

                switch (direction)
                {
                    case 0:
                        movingDirection = Vector2Int.up;
                        break;
                    case 1:
                        movingDirection = Vector2Int.left;
                        break;
                    case 2:
                        movingDirection = Vector2Int.down;
                        break;
                    case 3:
                        movingDirection = Vector2Int.right;
                        break;
                    case 4:
                        movingDirection = favourDirection;
                        break;
                    default:
                        break;
                }
            }

            pos = pos + movingDirection;
            endlessCheck++;
            if (endlessCheck == 100000)
                break;
            if (pos.x < mapSize.x && pos.x >= 0 &&
                pos.y < mapSize.y && pos.y >= 0)
            {
                index = Andre.Utils.ArrayUtils.ArrayIndex2DTo1D(pos.x, pos.y, mapSize.x);
                if (riverSize > 1)
                    DrawCircle(pos, riverSize - 1);
                else
                    layerData[index] = true;
            }
            else
            {
                toBuild = false;
                //pos = pos + -movingDirection;
            }
        }
    }

    Vector2Int initializeRandomStartLocation (int startSide)
    {
        float size = mapSize.x;
        if (startSide == 0)
            //up
            return new Vector2Int(Andre.Utils.MathUtils.RandomEqualChanceInt(1, mapSize.x - 1), mapSize.y - 1);
        //return new Vector2Int(Andre.Utils.MathUtils.RandomEqualChanceInt(1,mapSize.x - 1), 0);
        else if(startSide == 1)
            //Left
            return new Vector2Int(0, Andre.Utils.MathUtils.RandomEqualChanceInt(1, mapSize.y - 1));
        else if (startSide == 2)
            //down
            return new Vector2Int(Andre.Utils.MathUtils.RandomEqualChanceInt(1, mapSize.x - 1), 0);
        // return new Vector2Int(Andre.Utils.MathUtils.RandomEqualChanceInt(1,mapSize.x-1), mapSize.y-1);
        else
            //right
            return new Vector2Int(mapSize.x-1, Andre.Utils.MathUtils.RandomEqualChanceInt(1, mapSize.y - 1));
    }

    Vector2Int initializeStartLocation(int startSide)
    {
        float size = mapSize.x;
        if (startSide == 0)
            //up
            return new Vector2Int(Mathf.FloorToInt(mapSize.x / 2), mapSize.y - 1);
        //return new Vector2Int(Mathf.FloorToInt(mapSize.x / 2), 0);
        else if (startSide == 1)
            //Left
            return new Vector2Int(0, Mathf.FloorToInt(mapSize.y / 2));
        else if (startSide == 2)
            //down
            return new Vector2Int(Mathf.FloorToInt(mapSize.x / 2), 0);
        //return new Vector2Int(Mathf.FloorToInt(mapSize.x / 2), mapSize.y - 1);
        else
            //right
            return new Vector2Int(mapSize.x - 1, Mathf.FloorToInt(mapSize.y / 2));
    }
}
