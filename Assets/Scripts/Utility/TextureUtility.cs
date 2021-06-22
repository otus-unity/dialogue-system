using UnityEngine;

public static class TextureUtility
{
    public static Texture2D CreateSolidColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    public static Texture2D CreateTexture(int width, int height, Color[] color)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++)
                texture.SetPixel(x, height - y - 1, color[y * width + x]);
        }
        texture.Apply();
        return texture;
    }
}
