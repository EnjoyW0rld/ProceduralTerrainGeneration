using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class VoronoiMathematics : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float threshold = .5f;
    [SerializeField] private int scale;
    [SerializeField] private float divider = 1f;
    private void Start()
    {
        Texture2D tex = new Texture2D(scale, scale);
        Renderer rend = GetComponent<Renderer>();
        for (int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    float2 a = noise.cellular2x2x2(new float3(x/divider, y/divider, z/divider));
                    //a.x = 1 - a.x;
                    if(a.x > threshold) Instantiate(prefab,new Vector3(x,y,z),Quaternion.identity);
                    //tex.SetPixel(x, y, new Color(a.x, a.x, a.x));
                }
            }
        }
        //tex.Apply();
        //rend.material.mainTexture = tex;
    }
}