using UnityEngine;

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

    public static bool Approximately(Vector3 a, Vector3 b, float tolerance)
    {
        return (Mathf.Abs(a.x - b.x) < tolerance)
            && (Mathf.Abs(a.y - b.y) < tolerance)
            && (Mathf.Abs(a.z - b.z) < tolerance);
    }

    public static T[] GetEnumArray<T>()
    {
        T[] _enumValues = (T[])System.Enum.GetValues(typeof(T));
        return _enumValues;
    }
}