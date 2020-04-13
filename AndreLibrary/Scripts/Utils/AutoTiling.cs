using System;
using System.Collections.Generic;

namespace Andre.Utils
{
    [Flags]
    public enum DirectionsEightBit
    {
        NorthWest = 1 << 0,
        North = 1 << 1,
        NorthEast = 1 << 2,
        West = 1 << 3,
        East = 1 << 4,
        SouthWest = 1 << 5,
        South = 1 << 6,
        SouthEast = 1 << 7,
    }

    [Flags]
    public enum DirectionsFourBit
    {
        North = 1 << 0, // = 1
        West = 1 << 1, // = 2
        East = 1 << 2, // = 4
        South = 1 << 3, // = 8
    }

    public static class AutoTiling
    {
        public static Dictionary<int, int> EightBitConversionDic;
        //{ 2 = 1, 8 = 2, 10 = 3, 11 = 4, 16 = 5, 18 = 6, 22 = 7, 24 = 8, 26 = 9, 27 = 10, 30 = 11, 
        // 31 = 12, 64 = 13, 66 = 14, 72 = 15, 74 = 16, 75 = 17, 80 = 18, 82 = 19, 86 = 20, 88 = 21,
        // 90 = 22, 91 = 23, 94 = 24, 95 = 25, 104 = 26, 106 = 27, 107 = 28, 120 = 29, 122 = 30, 123 = 31, 
        // 126 = 32, 127 = 33, 208 = 34, 210 = 35, 214 = 36, 216 = 37, 218 = 38, 219 = 39, 222 = 40, 223 = 41,
        // 248 = 42, 250 = 43, 251 = 44, 254 = 45, 255 = 46, 0 = 47 }
        // https://gamedevelopment.tutsplus.com/tutorials/how-to-use-tile-bitmasking-to-auto-tile-your-level-layouts--cms-25673
        public static void InitConversionDictionary()
        {
            if (EightBitConversionDic != null)
                return;

            EightBitConversionDic = new Dictionary<int, int>();
            EightBitConversionDic.Add(2, 1);
            EightBitConversionDic.Add(8, 2);
            EightBitConversionDic.Add(10, 3);
            EightBitConversionDic.Add(11, 4);
            EightBitConversionDic.Add(16, 5);
            EightBitConversionDic.Add(18, 6);
            EightBitConversionDic.Add(22, 7);
            EightBitConversionDic.Add(24, 8);
            EightBitConversionDic.Add(26, 9);
            EightBitConversionDic.Add(27, 10);
            EightBitConversionDic.Add(30, 11);
            EightBitConversionDic.Add(31, 12);
            EightBitConversionDic.Add(64, 13);
            EightBitConversionDic.Add(66, 14);
            EightBitConversionDic.Add(72, 15);
            EightBitConversionDic.Add(74, 16);
            EightBitConversionDic.Add(75, 17);
            EightBitConversionDic.Add(80, 18);
            EightBitConversionDic.Add(82, 19);
            EightBitConversionDic.Add(86, 20);
            EightBitConversionDic.Add(88, 21);
            EightBitConversionDic.Add(90, 22);
            EightBitConversionDic.Add(91, 23);
            EightBitConversionDic.Add(94, 24);
            EightBitConversionDic.Add(95, 25);
            EightBitConversionDic.Add(104, 26);
            EightBitConversionDic.Add(106, 27);
            EightBitConversionDic.Add(107, 28);
            EightBitConversionDic.Add(120, 29);
            EightBitConversionDic.Add(122, 30);
            EightBitConversionDic.Add(123, 31);
            EightBitConversionDic.Add(126, 32);
            EightBitConversionDic.Add(127, 33);
            EightBitConversionDic.Add(208, 34);
            EightBitConversionDic.Add(210, 35);
            EightBitConversionDic.Add(214, 36);
            EightBitConversionDic.Add(216, 37);
            EightBitConversionDic.Add(218, 38);
            EightBitConversionDic.Add(219, 39);
            EightBitConversionDic.Add(222, 40);
            EightBitConversionDic.Add(223, 41);
            EightBitConversionDic.Add(248, 42);
            EightBitConversionDic.Add(250, 43);
            EightBitConversionDic.Add(251, 44);
            EightBitConversionDic.Add(254, 45);
            EightBitConversionDic.Add(255, 46);
            EightBitConversionDic.Add(0, 47);
        }

        public static DirectionsEightBit CalculateTileFlags8Bit(bool east, bool west, bool north, bool south, bool northWest, bool northEast, bool southWest, bool southEast)
        {
            DirectionsEightBit directions = (east ? DirectionsEightBit.East : 0) | (west ? DirectionsEightBit.West : 0) | (north ? DirectionsEightBit.North : 0) | (south ? DirectionsEightBit.South : 0);
            directions |= ((north && west) && northWest) ? DirectionsEightBit.NorthWest : 0;
            directions |= ((north && east) && northEast) ? DirectionsEightBit.NorthEast : 0;
            directions |= ((south && west) && southWest) ? DirectionsEightBit.SouthWest : 0;
            directions |= ((south && east) && southEast) ? DirectionsEightBit.SouthEast : 0;
            return directions;
        }


        public static int TileEightBitStandardConversion(DirectionsEightBit directions)
        {
            InitConversionDictionary();
            return EightBitConversionDic[(int)directions];
        }

        public static DirectionsFourBit CalculateTileFlags4Bit(bool east, bool west, bool north, bool south)
        {
            DirectionsFourBit directions = (east ? DirectionsFourBit.East : 0) | (west ? DirectionsFourBit.West : 0) | (north ? DirectionsFourBit.North : 0) | (south ? DirectionsFourBit.South : 0);
            return directions;
        }
    }
}
