using UnityEngine;

namespace Andre.Utils.RandomGeneration
{
    public static class DrunkenWalkGen
    {
        public static float DrunkSteps = 400;
        public static int NumberOfDrunks = 6;
        public static Vector3 drunkExit;
        public static Vector3 drunkEntry = Vector3.zero;

        public static int height;
        public static int width;
        public static Vector2 Padding = Vector2.zero;
        public static Color wallColour;
        public static Color groundColour;

        public static Texture2D DrunkWalkGenMap(Vector3 worldPos)
        {
            Texture2D map = new Texture2D(width, height, TextureFormat.ARGB32, false);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map.SetPixel(x, y, wallColour);
                }
            }

            DrunkenWalkGen.MoveDrunk(map,NumberOfDrunks, worldPos);
            return map;
        }

        public static void MoveDrunk(Texture2D map, int loops, Vector3 worldPos)
        {
            Vector3 Initpos = new Vector3(worldPos.x + Random.Range(2, width - 2), worldPos.y + Random.Range(2, height - 2), worldPos.z);
            for (int i = 0; i < loops; i++)
            {
                Vector3 pos = Initpos;
                map.SetPixel((int)pos.x, (int)pos.y, groundColour);
                float changeDirectionOdds = 0.8f;
                Vector3 movingDirection = Vector3.up;
                for (int s = 0; s < DrunkSteps; s++)
                {
                    //m_seedManager.ChangeSeed();
                    float chance = Random.Range(0.0f, 1.0f);
                    if (chance >= changeDirectionOdds)
                    {
                        //m_seedManager.ChangeSeed();
                        int direction = Mathf.FloorToInt(Random.value * 3.99f);
                        switch (direction)
                        {
                            case 0:
                                movingDirection = Vector3.up;
                                break;
                            case 1:
                                movingDirection = Vector3.left;
                                break;
                            case 2:
                                movingDirection = Vector3.down;
                                break;
                            case 3:
                                movingDirection = Vector3.right;
                                break;
                            default:
                                break;
                        }
                    }

                    pos += movingDirection;
                    if (pos.x < (worldPos.x + width) - Padding.x && pos.x > worldPos.x &&
                      pos.y < (worldPos.y + height) - Padding.y && pos.y > worldPos.y)
                    {
                        map.SetPixel((int)pos.x, (int)pos.y, groundColour);
                        if (pos.y > drunkExit.y)
                        {
                            drunkExit = pos;
                        }

                        if (pos.y < drunkEntry.y)
                        {
                            drunkEntry = pos;
                        }
                    }
                    else
                    {
                        pos += -movingDirection;
                    }
                }
            }
        }

        public static void CleanMap(Texture2D map)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = height - 1; y > -1; y--)
                {
                    map.SetPixel(x, y, Color.white);
                }
            }
        }
    }
}
