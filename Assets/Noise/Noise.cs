using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UtilityComputation
{
    public static Vector2 Rand22(int x, int y)
    {
        return new Vector2(Rand21(y, x * 25), Rand21(x, y * 13));
    }
    public static float Rand21(int x, int y)
    {
        Vector2 temp = new Vector2(x, y);
        return Fract(Mathf.Sin((Vector2.Dot(temp, new Vector2(32.5122f, 74.64732f)))) * 4543.222f);
    }
    public static float Fract(float val) => val - Mathf.Floor(val);
}

/// <summary>
/// Fractal Brownian motion algorithm general script
/// </summary>
[System.Serializable]
public class Fbm
{
    [SerializeField, Min(1)] private int octaves = 6;
    [SerializeField, Min(1)] private int upScaleNoise = 4;
    public void DrawNoiseToTex(int res, Renderer rend)
    {
        Texture2D tex = new Texture2D(res, res);
        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float val = 0;
                float amp = .5f;
                float[] cols = new float[3];
                for (int i = 0; i < octaves; i++)
                {
                    float octVal = amp * Mathf.PerlinNoise(x / (float)tex.width * upScaleNoise * (i * 2), y / (float)tex.height * upScaleNoise * (i * 2));

                    val += octVal;
                    cols[i % 3] += octVal;

                    amp *= .5f;
                }
                tex.SetPixel(x, y,
                    new Color(val, val, val)
                    //new Color(3*cols[0], 3*cols[1], 3*cols[2])
                    );
            }

        }
        tex.Apply();
        rend.material.mainTexture = tex;
    }
    public static float GetValue(float x, float y, int octaves, float scale)
    {
        float val = 0;
        float amp = .5f;
        for (int i = 0; i < octaves; i++)
        {
            val += amp * Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(x * scale * (i * 2), y * scale * (i * 2)));
            amp *= .2f;
        }
        return val;
    }
    [System.Obsolete]
    public static float GetValueMathfNoise(float x, float y, int octaves, float scale)
    {
        float val = 0;
        float amp = .5f;
        for (int i = 0; i < octaves; i++)
        {
            val += amp * Mathf.PerlinNoise(x * scale * (i * 2), y * scale * (i * 2));
            amp *= .5f;
        }
        if (val < 0 || val > 1) Debug.Log("AA");
        return val;
    }

}
