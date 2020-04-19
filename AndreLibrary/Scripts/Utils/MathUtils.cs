using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.Utils
{
    public static class MathUtils 
    {
        public static float RandomEqualChanceFloat(float min,float max)
        {
            if (min >= max)
                return 0;
            return min + (Random.value * ((max - min)- 0.01f));
        }

        public static float RandomEqualChanceFloat(float max)
        {
            return RandomEqualChanceFloat(0, max);
        }

        public static float RandomEqualChanceFloat(int min, int max)
        {
            float maxTmp = max;
            float minTmp = min;
            return RandomEqualChanceFloat(minTmp, maxTmp);
        }

        public static float RandomEqualChanceFloat(int max)
        {
            float maxTmp = max;
            return RandomEqualChanceFloat(0, maxTmp);
        }

        public static int RandomEqualChanceInt(float min, float max)
        {
            return Mathf.FloorToInt(RandomEqualChanceFloat(min, max));
        }

        public static int RandomEqualChanceInt(float max)
        {
            return Mathf.FloorToInt(RandomEqualChanceFloat(0, max));
        }

        public static int RandomEqualChanceInt(int min, int max)
        {
            float maxTmp = max;
            float minTmp = min;
            return Mathf.FloorToInt(RandomEqualChanceFloat(minTmp, maxTmp));
        }

        public static int RandomEqualChanceInt(int max)
        {
            float maxTmp = max;
            return Mathf.FloorToInt(RandomEqualChanceFloat(0, maxTmp));
        }
    }
}
