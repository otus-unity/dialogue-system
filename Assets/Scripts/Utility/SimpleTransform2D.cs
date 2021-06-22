using System;
using UnityEngine;

[Serializable]
public class SimpleTransform2D
{
    public Vector2 Offset = Vector2.zero;
    public float Scale = 1.0f;

    public void AdjustScale(float adjust, Vector2 center)
    {
        Offset += center * Scale;
        Scale -= adjust;
        if (Scale < 0.1f)
            Scale = 0.1f;
        Offset -= center * Scale;
    }
}
