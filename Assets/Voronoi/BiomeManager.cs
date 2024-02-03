using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to create a mask map from voronoi grid
/// </summary>
[System.Serializable]
public class BiomeManager : VoronoiBiomeSampler
{
    [Min(1)] public readonly int BiomeCount = 2;
    private Texture2D sampler, floatSampler;
    /// <summary>
    /// Creates and blures Texture2D for further sampling
    /// </summary>
    /// <param name="width">Width of the target texture</param>
    /// <param name="length">Length of the target texture</param>
    /// <param name="pointCount">Amount of sampled points, more give higher level of detail</param>
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
        Blur blur = new Blur();
        floatSampler = blur.ApplyBlur(floatSampler, 5, 3);
    }
    public Texture2D GetTex() => floatSampler;
    /// <summary>
    /// Get value of the biome at point
    /// </summary>
    /// <param name="sample">Point to sample, y coordinate is not being used</param>
    /// <returns>Value from 0 to 1 including</returns>
    public float GetBiomeFloatNum(Vector3 sample)
    {
        return floatSampler.GetPixel((int)sample.x, (int)sample.z).r;
    }
    /// <summary>
    /// Get value of the biome at point
    /// </summary>
    /// <param name="sample">Point to sample, y coordinate is not being used</param>
    /// <returns>Value 0 or 1</returns>
    public override int GetBiomeNum(Vector3 sample)
    {
        return (int)(sampler.GetPixel((int)sample.x, (int)sample.z).r);
    }
}