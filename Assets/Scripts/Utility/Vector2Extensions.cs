using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 InverseTransformedBy(this Vector2 point, SimpleTransform2D transform)
    {
        return (point - transform.Offset) / transform.Scale;
    }
}
