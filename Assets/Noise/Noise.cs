using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Noise : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] private int scale;
    [SerializeField] private float tempNum;
    enum NoiseType { Fbm, Perlin, Worley, TestCase }
    [SerializeField] private NoiseType noiseType;

    [SerializeField, HideIf("noiseType", NoiseType.Fbm, HideIfAttribute.Comparison.NotEquals)] private Fbm fbm;
    [SerializeField, HideIf("noiseType", NoiseType.Perlin, HideIfAttribute.Comparison.NotEquals)] private PerlinNoise perlinNoise;
    [SerializeField, HideIf("noiseType", NoiseType.Worley, HideIfAttribute.Comparison.NotEquals)] private Worley worleyNoise;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        ApplyAll();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyAll();
        }
    }
    private void TiledTexture(int res, Renderer rend)
    {
        Texture2D tex = new Texture2D(res, res);
        int tiledRes = res * 3;
        for (int x = 0; x < tiledRes; x++)
        {

            for (int y = 0; y < tiledRes; y++)
            {
                float val = Mathf.Lerp(0, 1, y / (float)tiledRes);
                tex.SetPixel(x, y, new Color(val, val, val));
            }
        }
        tex.Apply();
        rend.material.mainTexture = tex;
    }

    private void ApplyAll()
    {
        switch (noiseType)
        {
            case NoiseType.Fbm:
                fbm.DrawNoiseToTex(scale, rend);
                break;
            case NoiseType.Perlin:
                perlinNoise.DrawNoiseToTex(scale, rend);
                break;
            case NoiseType.Worley:
                worleyNoise.DrawNoiseToTex(scale, rend);
                break;
            case NoiseType.TestCase:
                TiledTexture(scale, rend);
                break;
        }
    }

}

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
            val += amp * Mathf.PerlinNoise(x * scale * (i * 2), y * scale * (i * 2));
            amp *= .5f;
        }
        return val;
    }
}
[System.Serializable]
internal class PerlinNoise
{
    [SerializeField, Min(1)] private int upScaleNoise = 3;
    public void DrawNoiseToTex(int res, Renderer rend)
    {
        Texture2D tex = new Texture2D(res, res);
        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float c = Mathf.PerlinNoise(x / (float)tex.width * upScaleNoise, y / (float)tex.height * upScaleNoise);
                tex.SetPixel(x, y, new Color(c, c, c));
            }
        }
        rend.material.mainTexture = tex;

        tex.Apply();
    }

}
// Technically same as Voronoi, not using it
[System.Serializable]
internal class Worley
{
    [SerializeField, Min(1)] private int pointsCount = 3;
    [SerializeField, Min(1)] private int modifier = 3;
    public void DrawNoiseToTex(int res, Renderer rend)
    {
        Texture2D tex = new Texture2D(res, res);
        res /= modifier;
        Vector2[] cellPos = new Vector2[pointsCount];
        // Placing random points on the map
        for (int i = 0; i < cellPos.Length; i++)
        {
            cellPos[i] = new Vector2(Random.Range(1, res) / (float)res, Random.Range(1, res) / (float)res);
        }

        for (int x = 0; x < res; x++)
        {
            // mapping position from 0 to 1
            float xPos = x / (float)res;
            for (int y = 0; y < res; y++)
            {
                float m_dist = 1;
                // mapping position from 0 to 1
                float yPos = y / (float)res;

                for (int i = 0; i < cellPos.Length; i++)
                {
                    // measuring distance from coordinate to point
                    float dist = Vector2.Distance(new Vector2(xPos, yPos), cellPos[i]);
                    m_dist = Mathf.Min(m_dist, dist);
                }
                tex.SetPixel(x, y, new Color(m_dist, m_dist, m_dist));
            }
        }
        tex.Apply();
        rend.material.mainTexture = tex;
    }
}

