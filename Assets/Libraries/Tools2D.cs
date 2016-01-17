using UnityEngine;

public abstract class Tools2D
{
    public static Vector2 Abs(Vector2 a)
    {
        return new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
    }

    public static Vector3 Add(Vector3 a, Vector2 b)
    {
        return a + (Vector3)b;
    }

    public static Vector2 Subtract(Vector2 a, Vector2 b)
    {
        return (a - b);
    }

    public static Vector2 Divide(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }

    public static Vector2 DirectionFromRotation(Quaternion rotation)
    {
        return rotation * Vector2.up;
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

    public static Vector3 MoveInDirection(Vector3 position, Vector2 direction, float t)
    {
        return new Vector3(position.x + (direction.x * t) , position.y + (direction.y * t), position.z);
    }

    public static Vector3 Multiply(Vector3 v, float mult)
    {
        return new Vector3(v.x * mult, v.y * mult, v.z);
    }

    public static bool SamePos(Vector2 a, Vector2 b, float threshold = 0)
    {
        if (threshold == 0)
        {
            return a.x == b.x && a.y == b.y;
        }
        else
        {
            return Mathf.Abs(a.x - b.x) < threshold && Mathf.Abs(a.y - b.y) < threshold;
        }      
    }

    public static bool Approximate(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance;
    }

    public static Quaternion LookAt(Vector2 position, Vector2 target)
    {
        Vector2 delta = Subtract(target, position);
        float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
        return Quaternion.Euler(0, 0, angle);
    }

    public static Quaternion LookAt(Vector2 direction)
    {
        Vector2 delta = direction.normalized;
        float angle = (delta.x < 0f) ? Vector2.Angle(Vector2.up, delta) : 360f - Vector2.Angle(Vector2.up, delta);
        return Quaternion.Euler(0, 0, angle);
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(a, b);
    }

    public static bool NotInside(Vector2 aBottomLeft, Vector2 aTopRight, Vector2 bBottomleft, Vector2 bTopRight)
    {
        return (aTopRight.x < bBottomleft.x) || (aTopRight.y < bBottomleft.y) || (aBottomLeft.x > bTopRight.x) || (aBottomLeft.y > bTopRight.y);
    }
}