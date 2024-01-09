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
    // Start is called before the first frame update
    void Start()
    {
        width *= SCALE;
        length *= SCALE;
        floatGrid = new float[width, maxDepth + maxElevation + ADDCEILING, length];
        tex = new Texture2D(width, length);

        //Generating cave
        int[,,] grid = new int[width, maxDepth + maxElevation, length];
        Add3DNoiseCaves(floatGrid, width, length, maxDepth);
        CaveLTree.CreateCave(floatGrid, new Vector3(width / 2, maxDepth - 1, length / 2), 30, 10, -1f, 2);

        //Surface creation
        GenerateBiomesWithNoiseSmoothed(floatGrid, width, length, maxElevation, maxDepth, ref tex);
        //GenerateBiomes3DSmoothed(floatGrid, width, length, maxElevation, maxDepth, ref tex);
        CaveLTree.DrawLine(floatGrid, new Vector3(width / 2, maxDepth, length / 2 - 1), new Vector3(width / 2, maxDepth + maxElevation, length / 2));
        Mesh mesh = GetSurcafeMesh();

        //combining meshes
        //CombineInstance[] meshes = CombineMeshes(mesh, caveMesh);

        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //finalMesh.CombineMeshes(meshes);


        //GetComponent<MeshFilter>().sharedMesh = MarchingCubes.GetMeshMarchingCubes(caveGrid,isoLevel,1);
        //GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = GenerateSurfaceAndCave();
        //GetComponent<MeshFilter>().sharedMesh = finalMesh;

        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.SetTexture("_Texture2D", tex);
        rend.sharedMaterial.SetFloat("_CaveHeight", maxDepth - 1);
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

        for (int x = 0; x < width; x++)
        {
            float realX = (x / (float)SCALE) / scaleVal + randVal; //mapped X value
            for (int z = 0; z < length; z++)
            {
                float realZ = (z / (float)SCALE) / scaleVal + randVal; //mapped Z value
                float biomeNum = biome.GetBiomeFloatNum(new Vector3(x, 0, z));
                //print(biome.GetBiomeFloatNum(new Vector3(x, 0, z)));
                float noiseValue;
                if (biomeNum == 1)
                {
                    noiseValue = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ));
                    //if (tex != null) tex.SetPixel(x, z, new Color(0, 0, 0));
                }
                else if (biomeNum == 0)
                {
                    noiseValue = Fbm.GetValue(realX, realZ, 6, 1);
                    //if (tex != null) tex.SetPixel(x, z, new Color(1, 1, 1));

                }
                else
                {
                    noiseValue = Mathf.Lerp(Fbm.GetValue(realX, realZ, 6, 1), Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float2(realX, realZ)), biomeNum);
                }

                for (int y = maxDepth; y < maxElevation + maxDepth + ADDCEILING; y++)
                {
                    //(y-maxDepth) (maxElevation - 2)
                    grid[x, y, z] = Mathf.Clamp01(((y - maxDepth) / ((float)maxElevation) + noiseValue) / 2f);
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
    private static Vector2[] BuildSurfaceUV(List<Vector3> vert, float width, float length)
    {
        Vector2[] uv = new Vector2[vert.Count];
        for (int i = 0; i < vert.Count; i++)
        {
            uv[i] = new Vector2(vert[i].x / width * SCALE, vert[i].z / length * SCALE);
        }
        return uv;
    }
    private static void Add3DNoiseCaves(float[,,] grid, int width,int length, int maxDepth)
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
                    grid[x,y,z] = Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(realX, realY, realZ));
                }
            }
        }
    }
}
