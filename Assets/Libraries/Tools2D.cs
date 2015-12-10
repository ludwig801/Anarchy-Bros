using UnityEngine;

namespace AnarchyBros
{
    public abstract class Tools2D
    {
        public static Vector3 AddKeepZ(Vector3 from, Vector3 to)
        {
            return from + new Vector3(to.x, to.y, 0);
        }

        public static Vector3 ConvertKeepZ(Vector3 from, Vector3 to)
        {
            return new Vector3(to.x, to.y, from.z);
        }

        public static bool IsPositionEqual(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }
}