using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi : MonoBehaviour
{
    [SerializeField, Min(1)] private int pointsCount;
    [SerializeField] private Bounds bounds;
    [SerializeField] private int scale;
    //private Vector3[] points;
    [SerializeField, Range(0, 1)] private float brightness = .5f;
    [SerializeField] private Dictionary<Vector3, Color> colourPoints;


    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // --- uncomment for gray texture ---
        rend.material.mainTexture = GenerateTexture(scale, scale, pointsCount);
        return;

        GenerateColouredTexture(rend);
    }


    private void GenerateColouredTexture(Renderer rend)
    {
        colourPoints = new Dictionary<Vector3, Color>();
        //points = new Vector3[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            //points[i] = new Vector3(Random.Range(-bounds.extents.x, bounds.extents.x), 0, Random.Range(-bounds.extents.z, bounds.extents.z));
            Vector3 randPoint = new Vector3(Random.Range(-bounds.extents.x, bounds.extents.x), 0, Random.Range(-bounds.extents.z, bounds.extents.z));
            Color c = Random.ColorHSV();
            colourPoints.Add(randPoint, c);
            //print(c);
        }

        Texture2D tex = new Texture2D(scale, scale);
        for (int x = 0; x < scale; x++)
        {
            float realX = x / (float)scale;
            for (int y = 0; y < scale; y++)
            {
                float realY = y / (float)scale;
                //tex.SetPixel(x, y, GetColour(new Vector3(realX * (float)bounds.size.x, 0, realY * bounds.size.z)));
                Vector3 lerpedCoords = new Vector3(Mathf.Lerp(bounds.extents.x, -bounds.extents.x, realX), 0, Mathf.Lerp(bounds.extents.z, -bounds.extents.z, realY));
                tex.SetPixel(x, y, GetColour(lerpedCoords));
            }
        }
        tex.Apply();
        rend.material.mainTexture = tex;
    }
    private Color GetColour(Vector3 pos)
    {
        KeyValuePair<Vector3, Color> a = new KeyValuePair<Vector3, Color>();
        bool initiated = false;
        foreach (var pair in colourPoints)
        {
            if (!initiated)
            {
                a = pair;
                initiated = true;
                continue;
            }
            if (Vector3.Distance(pair.Key, pos) < Vector3.Distance(a.Key, pos))
            {
                a = pair;
            }

        }
        print(a.Value);
        return a.Value;
    }

    private Texture2D GenerateTexture(int width, int height, int pointCount)
    {
        if (pointCount < 2) pointCount = 2;
        Texture2D tex = new Texture2D(width, height);
        Vector3[] points = new Vector3[pointCount];
        // generate mapped points
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector3(Random.Range(0, width) / (float)width, 0, Random.Range(0, height) / (float)height);
        }

        for (int x = 0; x < width; x++)
        {
            float realX = x / (float)width;
            for (int y = 0; y < height; y++)
            {
                float realY = y / (float)height;

                float closestPos = Vector3.Distance(new Vector3(realX, 0, realY), points[0]);
                for (int i = 1; i < points.Length; i++)
                {
                    float dist = Vector3.Distance(new Vector3(realX, 0, realY), points[i]);
                    closestPos = dist < closestPos ? dist : closestPos;
                }
                closestPos += brightness;
                closestPos = 1 - closestPos;
                
                tex.SetPixel(x, y, new Color(closestPos, closestPos, closestPos));
            }
        }
        tex.Apply();
        return tex;
    }

    private void OnDrawGizmos()
    {
        if (bounds != null)
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
        if (colourPoints != null)
        {
            foreach (var pair in colourPoints)
            {
                Gizmos.color = pair.Value;
                Gizmos.DrawSphere(pair.Key, .5f);
            }
        }

    }
}
