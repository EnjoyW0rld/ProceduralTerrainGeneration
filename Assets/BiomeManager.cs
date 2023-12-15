using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeManager : VoronoiBiomeSampler
{
    [Min(1)] public readonly int BiomeCount = 2;
    private Texture2D sampler, floatSampler;
    public BiomeManager(int width, int length, int pointCount = 5) : base(width, length, pointCount)
    {
        sampler = new Texture2D(width, length);
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                int colour = base.GetBiomeNum(new Vector3(x, 0, z)) % BiomeCount;
                
                sampler.SetPixel(x, z, new Color(colour, colour, colour));
            }
        }
        sampler.Apply();
        floatSampler = sampler;
        LinearBlur blur = new LinearBlur();
        floatSampler = blur.Blur(floatSampler, 5, 3);
    }
    //DEBUG
    public Texture2D GetTex() => floatSampler;
    public float GetBiomeFloatNum(Vector3 sample)
    {
        return floatSampler.GetPixel((int)sample.x, (int)sample.z).r;
    }
    public override int GetBiomeNum(Vector3 sample)
    {
        return (int)(sampler.GetPixel((int)sample.x, (int)sample.z).r);

        int res = base.GetBiomeNum(sample);
        return res % BiomeCount;
    }
}