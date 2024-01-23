using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothedTerrain : MonoBehaviour
{
    [SerializeField] private int width, length, maxElevation, maxDepth;
    [SerializeField, Range(0f, 1f)] private float isoLevel;
    private float[,,] floatGrid;
    const int SCALE = 3;
    const int ADDCEILING = 10;
    [SerializeField] private Texture2D tex;
    [SerializeField] private ComputeShader shader;
    [Header("Instancables")]
    [SerializeField] private GameObject bushPrefabDesert;
    [SerializeField] private GameObject bushPrefabGreen;
    // Start is called before the first frame update
    void Start()
    {
        width *= SCALE;
        length *= SCALE;
        floatGrid = new float[width, maxDepth + maxElevation + ADDCEILING, length];
        tex = new Texture2D(width, length);
        //Surface creation
        GenerateBiomesWithShader(ref tex);
        //GenerateBiomesWithNoiseSmoothed(floatGrid, width, length, maxElevation, maxDepth, ref tex);
        //GenerateBiomes3DSmoothed(floatGrid, width, length, maxElevation, maxDepth, ref tex);
        //Mesh mesh = GetSurcafeMesh();
        
        //Generating cave
        CaveLTree.CreateCave(floatGrid, new Vector3(width / 2, maxDepth - 1, length / 2), 30, 10, -1f, 2);

        CaveLTree.DrawLine(floatGrid, new Vector3(width / 2, maxDepth, length / 2 - 1), new Vector3(width / 2, maxDepth + maxElevation, length / 2));

        //finalMesh.CombineMeshes(meshes);

        
        Mesh finalMesh = GenerateSurfaceAndCave();
        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        //GetComponent<MeshFilter>().sharedMesh = finalMesh;

        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.SetTexture("_Texture2D", tex);
        rend.sharedMaterial.SetFloat("_CaveHeight", maxDepth - 1);

        GetComponent<MeshCollider>().sharedMesh = finalMesh;
        Stopwatch w = new Stopwatch();
        PropsPlacer.PlaceObjects(bushPrefabDesert,bushPrefabGreen, tex, 300,transform);
        w.print("placed");
        //print(MarchingCubes.GetPos(new Vector3(1, 14, 1), new Vector3(1, 14, 2), 0.2f, 0.3f, 0.25f));
    }

    private Mesh GenerateSurfaceAndCave()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();
        MarchingCubes.MarchAlgorithm(floatGrid, vertices, faces, isoLevel,
            delegate (int x, int y, int z)
            {
                if (y < maxDepth - 1) return new Vector3(SCALE, SCALE, SCALE);
                else return new Vector3(SCALE, 1, SCALE);
            });
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();
        mesh.uv2 = BuildSurfaceUV(vertices, width, length);

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();

        return mesh;
    }
    private Mesh GetSurcafeMesh()
    {
        Mesh mesh = new Mesh();
        MeshData data = MarchingCubes.GetMeshMarchingCubesData(floatGrid, isoLevel, SCALE);
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.faces.ToArray();
        //mesh.uv = BuildSurfaceUV(data.vertices, width, length);
        mesh.uv2 = BuildSurfaceUV(data.vertices, width, length);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        return mesh;
    }

    private CombineInstance[] CombineMeshes(Mesh mesh1, Mesh mesh2)
    {
        CombineInstance[] meshes = new CombineInstance[2];
        meshes[0].mesh = mesh1;
        meshes[0].transform = transform.localToWorldMatrix;
        meshes[1].mesh = mesh2;
        meshes[1].transform = transform.localToWorldMatrix;
        return meshes;
    }
    private static void GenerateBiomesWithNoiseSmoothed(float[,,] grid, int width, int length, int maxElevation, int maxDepth, ref Texture2D tex)
    {
        BiomeManager biome = new BiomeManager(width, length);
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;

        float[] temp = new float[width * (maxDepth + maxElevation + ADDCEILING) * length];

        for (int x = 0; x < width; x++)
        {
            float realX = (x / (float)SCALE) / scaleVal + randVal; //mapped X value
            for (int z = 0; z < length; z++)
            {
                float realZ = (z / (float)SCALE) / scaleVal + randVal; //mapped Z value
                float biomeNum = biome.GetBiomeFloatNum(new Vector3(x, 0, z));
                float noiseValue;
                if (biomeNum == 1)
                {
                    noiseValue = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ));
                }
                else if (biomeNum == 0)
                {
                    noiseValue = Fbm.GetValue(realX, realZ, 6, 1);
                }
                else
                {
                    noiseValue = Mathf.Lerp(Fbm.GetValue(realX, realZ, 6, 1), Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ)), biomeNum);
                }
                //Creating 3D caves
                for (int y = 0; y < maxDepth; y++)
                {
                    float realY = (y / (float)SCALE) / scaleVal + randVal; //mapped Z value
                    //grid[x, y, z] = Mathf.Clamp01(Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(realX, realY, realZ)) - .5f);
                }

                for (int y = maxDepth; y < maxElevation + maxDepth + ADDCEILING; y++)
                {
                    //grid[x, y, z] = Mathf.Clamp01(((y - maxDepth) / ((float)maxElevation) + noiseValue) / 2f);
                    temp[x + width * y + width * (maxElevation + maxDepth + ADDCEILING) * z] = Mathf.Clamp01(((y - maxDepth) / ((float)maxElevation) + noiseValue));
                }
            }

        }
        for (int x = 0; x < width; x++)
        {
            for (int y = maxDepth; y < (maxDepth + maxElevation + ADDCEILING); y++)
            {
                for (int z = 0; z < length; z++)
                {
                    grid[x, y, z] = temp[x + width * y + width * (maxDepth + maxElevation + ADDCEILING) * z];
                }
            }
        }

        tex = biome.GetTex();
        //tex.Apply();
    }
    private static void GenerateBiomes3DSmoothed(float[,,] grid, int width, int length, int maxElevation, int maxDepth, ref Texture2D tex)
    {
        BiomeManager biome = new BiomeManager(width, length);
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;

        for (int x = 0; x < width; x++)
        {
            float realX = (x / (float)SCALE) / scaleVal + randVal; //mapped X value
            for (int z = 0; z < length; z++)
            {
                float realZ = (z / (float)SCALE) / scaleVal + randVal; //mapped Z value
                for (int y = maxDepth; y < maxElevation + maxDepth + ADDCEILING; y++)
                {
                    float realY = (y / (float)SCALE) / scaleVal + randVal;
                    float verticalOffValue = Mathf.LerpUnclamped(0, 1, (y - maxDepth) / ((float)maxElevation));
                    if (x == 2 && z == 2) print(verticalOffValue);
                    grid[x, y, z] = Mathf.Clamp01(
                        verticalOffValue
                        - Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(realX, realY, realZ))
                        );
                }
            }
        }
    }
    private void GenerateBiomesWithShader(ref Texture2D tex)
    {

        BiomeManager biome = new BiomeManager(width, length);
        float[] tempGrid = new float[width * (maxDepth + maxElevation + ADDCEILING) * length];
        ComputeBuffer buffer = new ComputeBuffer(width * (maxDepth + maxElevation + ADDCEILING) * length, sizeof(float));
        buffer.SetData(tempGrid);
        shader.SetTexture(0, "biomes", biome.GetTex());
        shader.SetFloat("maxDepth", maxDepth);
        shader.SetFloat("maxElevation", maxElevation);
        shader.SetVector("gridSize", new Vector3(width, (maxDepth + maxElevation + ADDCEILING), length));
        shader.SetBuffer(0, "grid", buffer);
        shader.Dispatch(0, width / 10, 1, length / 10);
        buffer.GetData(tempGrid);
        buffer.Dispose();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < (maxDepth + maxElevation + ADDCEILING); y++)
            {
                for (int z = 0; z < length; z++)
                {
                    floatGrid[x, y, z] = tempGrid[x + width * y + width * (maxDepth + maxElevation + ADDCEILING) * z];
                }
            }
        }
        tex = biome.GetTex();

    }
    private void debug(ref Texture2D tex)
    {
        float[] tempGrid = new float[width * (maxDepth + maxElevation + ADDCEILING) * length];
        for (int x = 0; x < width; x++)
        {
            float realX = (x / (float)3.0f) / 10.0f;
            for (int z = 0; z < length; z++)
            {
                float realZ = (z / (float)3.0f) / 10.0f;
                float noiseVal = Fbm.GetValue(realX, realZ, 6, 1);
                //float noiseVal = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ)); ;

                for (int y = maxDepth; y < (maxDepth + maxElevation + ADDCEILING); y++)
                {
                    tempGrid[x + width * y + width * (maxDepth + maxElevation + ADDCEILING) * z] =
                        Mathf.Clamp01(((y - maxDepth) / (float)maxElevation + noiseVal) / 2.0f);
                }
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = maxDepth; y < (maxDepth + maxElevation + ADDCEILING); y++)
            {
                for (int z = 0; z < length; z++)
                {
                    floatGrid[x, y, z] = tempGrid[x + width * y + width * (maxDepth + maxElevation + ADDCEILING) * z];
                }
            }
        }

    }

    private static Vector2[] BuildSurfaceUV(List<Vector3> vert, float width, float length)
    {
        Vector2[] uv = new Vector2[vert.Count];
        for (int i = 0; i < vert.Count; i++)
        {
            uv[i] = new Vector2(vert[i].x / width * SCALE, vert[i].z / length * SCALE);
        }
        return uv;
    }
    private static void Add3DNoiseCaves(float[,,] grid, int width, int length, int maxDepth)
    {
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;

        for (int x = 0; x < width; x++)
        {
            float realX = (x / (float)SCALE) / scaleVal + randVal; //mapped X value
            for (int z = 0; z < length; z++)
            {
                float realZ = (z / (float)SCALE) / scaleVal + randVal; //mapped Z value
                for (int y = 0; y < maxDepth; y++)
                {
                    float realY = (y / (float)SCALE) / scaleVal + randVal; //mapped Z value
                    grid[x, y, z] = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(realX, realY, realZ));
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector3(0, maxDepth, 0), Vector3.one);
    }
}
public class Stopwatch
{
    System.Diagnostics.Stopwatch sw;
    public Stopwatch()
    {
        sw = new System.Diagnostics.Stopwatch();
        sw.Start();
    }
    public void print(string text)
    {
        sw.Stop();
        Debug.Log(text + " " + sw.ElapsedMilliseconds / 1000f);
    }
}