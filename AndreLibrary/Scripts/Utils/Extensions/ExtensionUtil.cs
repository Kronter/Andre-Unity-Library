using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ExtensionUtil
{
    public static Vector3 DirectionTo(this Vector3 A, Vector3 B)
    {
        return B - A;
    }

    public static Vector2 DirectionTo(this Vector2 A, Vector2 B)
    {
        return B - A;
    }

    public static Color WithAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static Vector3 WithMagnitude(this Vector3 v, float magnitude)
    {
        return v.normalized * magnitude;
    }

    public static Vector2 WithMagnitude(this Vector2 v, float magnitude)
    {
        return v.normalized * magnitude;
    }

    public static Vector3 WithY(this Vector3 v, float newY)
    {
        return new Vector3(v.x, newY, v.z);
    }

    public static Vector3 WithX(this Vector3 v, float newX)
    {
        return new Vector3(newX, v.y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float newZ)
    {
        return new Vector3(v.x, v.y, newZ);
    }

    public static Vector2 WithY(this Vector2 v, float newY)
    {
        return new Vector2(v.x, newY);
    }

    public static Vector2 WithX(this Vector2 v, float newX)
    {
        return new Vector2(newX, v.y);
    }

    public static Vector3 PixelPerfectMoveClamp(this Vector3 moveVector, float pixelPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(
                Mathf.RoundToInt(moveVector.x * pixelPerUnit),
                Mathf.RoundToInt(moveVector.y * pixelPerUnit),
                Mathf.RoundToInt(moveVector.z * pixelPerUnit));
        return vectorInPixels / pixelPerUnit;
    }

    public static Vector2 PixelPerfectMoveClamp(this Vector2 moveVector, float pixelPerUnit)
    {
        Vector2 vectorInPixels = new Vector2(
                Mathf.RoundToInt(moveVector.x * pixelPerUnit),
                Mathf.RoundToInt(moveVector.y * pixelPerUnit));
        return vectorInPixels / pixelPerUnit;
    }

    public static string CharArrayToString(this char[] charArray)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in charArray)
        {
            sb.Append(c);
        }

        return sb.ToString();
    }

    public static string LimitStringLength(this string sentence, int totalLength, char[] charactersToAddNewLine)
    {
        StringBuilder sb = new StringBuilder();

        int currentCount = 0;
        foreach (char c in sentence)
        {
            if (currentCount >= totalLength)
            {
                if (charactersToAddNewLine.Contains(c))
                {
                    sb.Append('\n');
                    sb.Append(c);
                    currentCount = 0;
                }
                else
                {
                    sb.Append(c);
                }
            }
            else
            {
                sb.Append(c);
                currentCount++;
            }
        }

        return sb.ToString();
    }

    public static string LimitStringLength(this string sentence, int totalLength)
    {
        StringBuilder sb = new StringBuilder();

        int currentCount = 0;
        foreach (char c in sentence)
        {
            if (currentCount >= totalLength)
            {
                sb.Append('\n');
                sb.Append(c);
                currentCount = 0;
            }
            else
            {
                sb.Append(c);
                currentCount++;
            }
        }

        return sb.ToString();
    }

    private static bool Contains(this char[] charactersCheck, char character)
    {
        foreach (char c in charactersCheck)
        {
            if (c == character)
            {
                return true;
            }
        }
        return false;
    }

    public static bool ListContainsByName(this List<Object> List, string name)
    {
        foreach (Object item in List)
        {
            if (item.name == name)
            {
                return true;
            }
        }
        return false;
    }

    public static float WeightedNormalizeToRange(this float numberToNormalize, float minScale, float maxScale,
            float minRange, float maxRange, float weight)
    {
        if (weight < 0)
        {
            return (maxScale - numberToNormalize) / (maxScale - minScale) * (maxRange - minRange) + minRange;
        }
        else
        {
            return (numberToNormalize - minScale) / (maxScale - minScale) * (maxRange - minRange) + minRange;
        }
    }

    public static float NormalizeToRange(this float numberToNormalize, float minScale, float maxScale,
        float minRange, float maxRange)
    {
        return WeightedNormalizeToRange(numberToNormalize, minScale, maxScale,
            minRange, maxRange, 1.0f);
    }


    public static float Normalize(this float numberToNormalize, float minScale, float maxScale)
    {
        return WeightedNormalizeToRange(numberToNormalize, minScale, maxScale,
            0.0f, 1.0f, 1.0f);
    }

    public static bool CheckIsColour(this Texture2D tex ,int _x, int _y, Vector2Int _dir, Color _color)
    {
        if (_x + _dir.x > tex.width-1)
            return false;
        if (_y + _dir.y > tex.height-1)
            return false;
        if (_x + _dir.x < 0)
            return false;
        if (_y + _dir.y < 0)
            return false;
        return tex.GetPixel(_x + _dir.x, _y + _dir.y) == _color;
    }
}
