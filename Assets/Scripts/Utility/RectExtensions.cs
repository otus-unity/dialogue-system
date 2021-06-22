using UnityEngine;

public static class RectExtensions
{
    public static Rect OffsetBy(this Rect rect, Vector2 offset)
    {
        return new Rect(rect.min + offset, rect.size);
    }

    public static Rect TransformedBy(this Rect rect, SimpleTransform2D transform)
    {
        return new Rect(rect.min * transform.Scale + transform.Offset, rect.size * transform.Scale);
    }
}
