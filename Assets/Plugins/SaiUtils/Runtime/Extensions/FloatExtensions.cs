using UnityEngine;

namespace SaiUtils.Extensions
{
    public static class FloatExtensions
    {
        public static bool WithinRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }

    public static class Vector2Extensions
    {
        public static float GetRandomValue(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }
    }

    public static class Vector3Extensions
    {
        public static float Distance(this Vector3 a, Vector3 b, bool ignoreY = false, bool ignoreZ = false, bool ignoreX = false)
        {
            if (ignoreY) b.y = a.y;
            if (ignoreZ) b.z = a.z;
            if (ignoreX) b.x = a.x;

            return Vector3.Distance(a, b);
        }
    }
}