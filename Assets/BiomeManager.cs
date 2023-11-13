using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeManager : VoronoiBiomeSampler
{
    [Min(1)] public readonly int BiomeCount = 2;

    public BiomeManager(int width, int length, int pointCount = 5) : base(width, length, pointCount)
    {
    }
    public override int GetBiomeNum(Vector3 sample)
    {
        int res = base.GetBiomeNum(sample);
        return res % BiomeCount;
    }
}
[System.Serializable]
public class BiomeData
{

}