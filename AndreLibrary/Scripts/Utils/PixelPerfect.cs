using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.Utils.Pixels
{
    public static class Math
    {
        public static Vector3 PixelPerfectMoveClamp(Vector3 moveVector, float pixelPerUnit)
        {
            Vector3 vectorInPixels = new Vector3(
                    Mathf.RoundToInt(moveVector.x * pixelPerUnit),
                    Mathf.RoundToInt(moveVector.y * pixelPerUnit),
                    Mathf.RoundToInt(moveVector.z * pixelPerUnit));
            return vectorInPixels / pixelPerUnit;
        }

        public static Vector2 PixelPerfectMoveClamp(Vector2 moveVector, float pixelPerUnit)
        {
            Vector2 vectorInPixels = new Vector2(
                    Mathf.RoundToInt(moveVector.x * pixelPerUnit),
                    Mathf.RoundToInt(moveVector.y * pixelPerUnit));
            return vectorInPixels / pixelPerUnit;
        }
    }
}
