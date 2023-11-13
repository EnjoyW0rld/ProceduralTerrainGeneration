using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PlaneBuilderWithMesh : MonoBehaviour
{
    [SerializeField] private int maxHeight = 6;
    [SerializeField] private int width, length;
    [SerializeField] private int[,,] p;
    [SerializeField] private float threshold = .5f;

    [Header("Ground and caves")]
    [SerializeField] private int maxDepth = 20;
    [SerializeField] private int maxElevation = 10;
    [SerializeField, Range(0f, 1f)] private float caveThreshold = .5f;

    [SerializeField] private Texture2D tex;

    //[SerializeField] private BiomeManager biomeManager;// = new BiomeManager();

    private void Start()
    {
        MeshFilter rend = GetComponent<MeshFilter>();
        //MeshData data = GenerateBiomes();
        //MeshData data = GenerateGround();
        MeshData data = GenerateBiomesWithNoises();
        
        Mesh mesh = new Mesh();

        mesh.SetVertices(data.vertices);
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.triangles = data.faces.ToArray();

        mesh.RecalculateNormals();

        rend.mesh = mesh;

        mesh.uv = BuildSurfaceUV(data.vertices);
      
        //return;
        //SETTING TEXTURE TO SHADER
        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Texture2D", tex);
    }


    private MeshData GenerateGround()
    {
        //MeshData data;
        int[,,] places = new int[width, maxDepth + maxElevation, length];
        //float randVal = 0;
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;

        for (int x = 0; x < width; x++)
        {
            float realX = x / scaleVal + randVal;
            for (int z = 0; z < length; z++)
            {
                float realZ = z / scaleVal + randVal;

                float noiseValue = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ));

                int mappedHeight = (int)Mathf.Lerp(maxDepth, maxElevation + maxDepth, noiseValue);
                for (int y = mappedHeight; y > maxDepth; y--)
                {
                    places[x, y, z] = 1;
                }
                places[x, mappedHeight, z] = 1;
            }
        }
        p = places;


        // CAVE GENERATION
        for (int x = 0; x < width; x++)
        {
            float realX = x / scaleVal + randVal;
            for (int y = 0; y < maxDepth + 5; y++)
            {
                float realY = y / scaleVal + randVal;
                for (int z = 0; z < length; z++)
                {
                    float realZ = (z / scaleVal) + randVal;

                    float val = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(realX, realY, realZ));
                    if (val < caveThreshold)
                    {
                        places[x, y, z] = 1;
                    }
                }
            }
        }


        MeshData data = MarchingCubes.GetDataMarchingCubes(places);
        return data;


    }
    private MeshData GenerateBiomes()
    {
        BiomeManager biome = new BiomeManager(width, length);

        int[,,] places = new int[width, maxDepth + maxElevation, length];
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;
        tex = new Texture2D(width, length); //Generating texture
        for (int x = 0; x < width; x++)
        {
            float realX = x / scaleVal + randVal; //mapped X value
            for (int z = 0; z < length; z++)
            {
                float realZ = z / scaleVal + randVal; //mapped Z value

                float noiseValue = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ));
                //Mapping height based on the biome number
                int biomeNum = biome.GetBiomeNum(new Vector3(x, 0, z));
                int currentMaxElevation = maxElevation / (biomeNum + 1);
                int mappedHeight = (int)Mathf.Lerp(maxDepth, currentMaxElevation + maxDepth, noiseValue);


                tex.SetPixel(x, z, new Color(biomeNum / 10f, biomeNum / 10f, biomeNum / 10f));

                for (int y = mappedHeight; y > maxDepth; y--)
                {
                    places[x, y, z] = 1;
                }
                places[x, mappedHeight, z] = 1;
            }
        }
        tex.Apply();
        MeshData data = MarchingCubes.GetDataMarchingCubes(places);

        return data;
    }
    private MeshData GenerateBiomesWithNoises()
    {
        BiomeManager biome = new BiomeManager(width, length);
        int[,,] places = new int[width, maxDepth + maxElevation, length];
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;
        tex = new Texture2D(width, length); //Generating texture


        for (int x = 0; x < width; x++)
        {
            float realX = x / scaleVal + randVal; //mapped X value
            for (int z = 0; z < length; z++)
            {
                float realZ = z / scaleVal + randVal; //mapped Z value
                int biomeNum = biome.GetBiomeNum(new Vector3(x, 0, z));
                float noiseValue;
                if (biomeNum == 1)
                {
                    noiseValue = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ));
                }
                else noiseValue = Fbm.GetValue(realX, realZ, 6, 1);

                int mappedHeight = (int)Mathf.Lerp(maxDepth, maxElevation + maxDepth, noiseValue);
                tex.SetPixel(x, z, new Color(biomeNum / 10f, biomeNum / 10f, biomeNum / 10f));
                for (int y = mappedHeight; y > maxDepth; y--)
                {
                    places[x, y, z] = 1;
                }
                places[x, mappedHeight, z] = 1;
            }

        }
        tex.Apply();
        MeshData data = MarchingCubes.GetDataMarchingCubes(places);

        return data;
    }
    private Vector2[] BuildSurfaceUV(List<Vector3> vert)
    {
        Vector2[] uv = new Vector2[vert.Count];
        for (int i = 0; i < vert.Count; i++)
        {
            uv[i] = new Vector2(vert[i].x / (float)width, vert[i].z / (float)length);
        }
        return uv;
    }
    
    private void OnDrawGizmos()
    {
        for (int y = 0; y < maxDepth + maxElevation; y++)
        {
            UnityEditor.Handles.Label(new Vector3(0, y, 0), y + "");
        }


        return;
        if (p == null) return;
        for (int x = 0; x < p.GetLength(0); x++)
        {
            for (int y = 0; y < p.GetLength(1); y++)
            {
                for (int z = 0; z < p.GetLength(2); z++)
                {
                    if (p[x, y, z] == 0) continue;
                    UnityEditor.Handles.Label(new Vector3(x, y, z), p[x, y, z] + "");
                }
            }
        }
    }
}