using UnityEngine;

namespace AnarchyBros
{
    public abstract class Tools2D
    {
        public static Vector3 Move(Vector3 from, Vector3 to)
        {
            return from + new Vector3(to.x, to.y, 0);
        }
    }
}