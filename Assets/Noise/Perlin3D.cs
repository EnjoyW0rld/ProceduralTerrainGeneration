using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin3D : MonoBehaviour
{
    [SerializeField] private Vector3 size;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float threshold = .5f;
    [SerializeField] private float scale = 1;
    private float randVal = 1;

    private void Start()
    {
        //randVal = System.DateTime.Now.Second;
        for (int x = 0; x < size.x; x++)
        {
            float Xmapped = x / scale;// size.x;
            for (int y = 0; y < size.y; y++)
            {
                float Ymapped = y / scale;// size.y;
                for (int z = 0; z < size.z; z++)
                {
                    float Zmapped = z / scale;// size.z;
                    Vector3 pos = new Vector3(Xmapped * randVal, Ymapped * randVal, Zmapped * randVal);
                    if (GetPerlin3D(pos) < threshold)
                    {
                        Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }
    }

    public static float GetPerlin3D(Vector3 val)
    {
        float ab = Mathf.PerlinNoise(val.x, val.y);
        float bc = Mathf.PerlinNoise(val.y, val.z);
        float ac = Mathf.PerlinNoise(val.x, val.z);

        float ba = Mathf.PerlinNoise(val.y, val.x);
        float cb = Mathf.PerlinNoise(val.z, val.y);
        float ca = Mathf.PerlinNoise(val.z, val.x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}