using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiBiomeSampler
{
    private Vector3[] points;
    public VoronoiBiomeSampler(int width, int length, int pointCount = 5)
    {
        points = new Vector3[pointCount];
        InitializePoints(width, length, pointCount);
    }
    private void InitializePoints(int width, int length, int pointCount)
    {
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(Random.Range(0, width), 0, Random.Range(0, length));
        }
    }
    /// <summary>
    /// Returns the number from 0 to the amount of sampled points
    /// </summary>
    /// <param name="sample"></param>
    /// <returns></returns>
    public virtual int GetBiomeNum(Vector3 sample)
    {
        float dist = Vector3.Distance(sample, points[0]);
        int resNum = 0;
        for (int i = 1; i < points.Length; i++)
        {
            float currentDist = Vector3.Distance(sample, points[i]);
            if (currentDist < dist)
            {
                resNum = i;
                dist = currentDist;
            }
        }
        return resNum;
    }

}
