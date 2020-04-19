using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.Utils
{
    public static class ArrayUtils 
    {
        public static int ArrayIndex2DTo1D(int x, int y, int width)
        {
            return x + (width * y);
        }

        public static Vector2Int ArrayPositionDTo1D(int i, int width)
        {
            return new Vector2Int(i% width, i/ width);
        }

        public static int ArrayIndex3DTo1D(int x, int y, int z, int width, int height)
        {
            return x + (width * y) + (width*height*z);
        }

        public static Vector3Int ArrayPosition3DTo1D(int i, int width, int height)
        {
            return new Vector3Int(i % width, (i / width)% height, i/(width*height));
        }
    }
}
