using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Mathematics;

public class Voronoi3D : MonoBehaviour
{
    [SerializeField] private int pointsAmount;
    [SerializeField] private Vector3 maxVolume;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float threshold;
    private Vector3[] points;
    private List<GameObject> instantiated = new List<GameObject>();
    private void Start()
    {
        GenerateAll();
        //Unity.Mathematics.noise.cellular()
    }
    private void GenerateAll()
    {
        points = new Vector3[pointsAmount];
        for (int i = 0; i < pointsAmount; i++)
        {
            points[i] = new Vector3(Random.value, Random.value, Random.value);
        }
        for (int x = 0; x < maxVolume.x; x++)
        {
            for (int y = 0; y < maxVolume.y; y++)
            {
                for (int z = 0; z < maxVolume.z; z++)
                {
                    if (getValue(new Vector3(x, y, z)) < threshold)
                    {
                        instantiated.Add(Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity));
                    }
                }
            }
        }
    }
    private float getValue(Vector3 vec)
    {
        float minDist = 1;
        Vector3 correctedVector = new Vector3(vec.x / maxVolume.x, vec.y / maxVolume.y, vec.z / maxVolume.z);
        for (int i = 0; i < pointsAmount; i++)
        {
            float dist = Vector3.Distance(points[i], correctedVector);
            if (dist < minDist) minDist = dist;
        }
        return minDist;
    }
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i < instantiated.Count; i++)
            {
                Destroy(instantiated[i].gameObject);
            }
            instantiated.Clear();
            GenerateAll();
        }
    }
}
