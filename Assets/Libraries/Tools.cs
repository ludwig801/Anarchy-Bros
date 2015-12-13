using UnityEngine;

namespace AnarchyBros
{
    public abstract class Tools
    {
        public static bool AreColorsEqual(Color colorA, Color colorB)
        {
            float r = Mathf.Abs(colorA.r - colorB.r);
            float g = Mathf.Abs(colorA.g - colorB.g);
            float b = Mathf.Abs(colorA.b - colorB.b);
            float a = Mathf.Abs(colorA.a - colorB.a);

            return Mathf.Approximately(r, 0) &&
                Mathf.Approximately(g, 0) &&
                Mathf.Approximately(b, 0) &&
                Mathf.Approximately(a, 0);
        }
    }
}