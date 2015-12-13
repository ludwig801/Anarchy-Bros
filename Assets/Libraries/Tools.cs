using UnityEngine;

namespace AnarchyBros
{
    public abstract class Tools
    {
        public static bool AreColorsEqual(Color colorA, Color colorB)
        {
            return Approximately(colorA.r, colorB.r) &&
                Approximately(colorA.g, colorB.g) &&
                Approximately(colorA.b, colorB.b) &&
                Approximately(colorA.a, colorB.a);
        }

        public static bool Approximately(float a, float b, float tolerance = 0.01f)
        {
            return (Mathf.Abs(a - b) < tolerance);
        }
    }
}