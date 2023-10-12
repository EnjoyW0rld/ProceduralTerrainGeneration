using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PlaneBuilderWithMesh : MonoBehaviour
{
    [SerializeField] private int maxHeight = 6;
    [SerializeField] private int width, length;
    [SerializeField] private int[,,] p;
    [SerializeField] private float threshold = .5f;


    private void Start()
    {
        MeshFilter rend = GetComponent<MeshFilter>();

        int[,,] val = new int[width, maxHeight, length];
        //print(val[1, 0, 2]);
        for (int x = 0; x < width; x++)
        {
            float realX = x / 10f;
            for (int y = 0; y < maxHeight; y++)
            {
                float realY = y / 10f;
                for (int z = 0; z < length; z++)
                {
                    float realZ = z / 10f;
                    float num = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(realX, realY, realZ));
                    if (num < threshold)
                    {
                        val[x, y, z] = 1;
                    }
                    //int y = (int)Mathf.Lerp(0,maxHeight, Mathf.PerlinNoise(realX, realZ));
                    //val[x, y, z] = 1;
                }
            }
        }
        p = val;
        rend.mesh = MarchingCubes.GetMeshMarchingCubes(val);
    }


    private void GenerateGround()
    {

    }
    private void Test(MeshFilter rend)
    {
        int[,,] val = new int[,,]
        {
            {{0,0,0,0 },{0,0,0,0 },{0,0,0,0 },{0,0,0,0 } },
            {{0,0,0,0 },{0,0,0,0 },{0,0,0,0 },{0,1,0,0 } },
            {{0,0,0,0 },{0,0,0,0 },{0,0,0,0 },{0,0,0,0 } },
            {{0,0,0,0 },{0,0,0,0 },{0,0,0,0 },{0,1,0,0 } }
        };
        p = val;
        rend.mesh = MarchingCubes.GetMeshMarchingCubes(val);
    }
    private void FillEmpty(int[,,] arr)
    {
        for (int x = 0; x < arr.GetLength(0); x++)
        {
            for (int y = 0; y < arr.GetLength(1); y++)
            {
                for (int z = 0; z < arr.GetLength(2); z++)
                {
                    //if (arr[x,y,z] == null)
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (p == null) return;
        for (int x = 0; x < p.GetLength(0); x++)
        {
            for (int y = 0; y < p.GetLength(1); y++)
            {
                for (int z = 0; z < p.GetLength(2); z++)
                {
                    UnityEditor.Handles.Label(new Vector3(x, y, z), p[x, y, z] + "");
                }
            }
        }
    }
}