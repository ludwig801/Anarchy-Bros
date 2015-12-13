using UnityEngine;

namespace AnarchyBros
{
    public abstract class Tools2D
    {
        public static Vector3 Add(Vector3 a, Vector2 b)
        {
            return a + (Vector3)b;
        }

        public static Vector3 Subtract(Vector3 a, Vector2 b)
        {
            return a - (Vector3)b;
        }

        public static Vector3 Convert(Vector3 from, Vector2 to)
        {
            return new Vector3(to.x, to.y, from.z);
        }

        public static Vector3 MoveTowards(Vector3 from, Vector2 to, float t)
        {
            Vector2 v2d = Vector2.MoveTowards(from, to, t);
            return new Vector3(v2d.x, v2d.y, from.z);
        }

        public static Vector3 Multiply(Vector3 v, float mult)
        {
            return new Vector3(v.x * mult, v.y * mult, v.z);
        }

        public static bool IsPositionEqual(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }
    }
}