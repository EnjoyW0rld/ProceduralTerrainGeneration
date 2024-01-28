using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider))]
public class SmoothedTerrain : MonoBehaviour
{
    const int SCALE = 1;
    const int ADDCEILING = 10;

    
    [Header("Terrain dimensions")]
    [SerializeField] private int width;
    [SerializeField] private int length, maxElevation, maxDepth;
    [SerializeField, Range(0f, 1f)] private float isoLevel;
    [Header("Shader variables")]
    [SerializeField] private Texture2D tex;
    [SerializeField] private ComputeShader shader;
    [Header("Instancables")]
    [SerializeField] private GameObject bushPrefabDesert;
    [SerializeField] private GameObject bushPrefabGreen;

    private float[,,] floatGrid;
    // Start is called before the first frame update
    void Start()
    {
        width *= SCALE;
        length *= SCALE;
        floatGrid = new float[width, maxDepth + maxElevation + ADDCEILING, length];
        tex = new Texture2D(width, length);
        //Surface creation
        GenerateBiomesWithShader(ref tex);
        
        //Generating cave
        CaveLTree.CreateCave(floatGrid, new Vector3(width / 2, maxDepth - 1, length / 2), 30, 10, -1f, 2);
        CaveLTree.DrawLine(floatGrid, new Vector3(width / 2, maxDepth, length / 2 - 1), new Vector3(width / 2, maxDepth + maxElevation, length / 2));
        
        Mesh finalMesh = GenerateSurfaceAndCave();

        GetComponent<MeshCollider>().sharedMesh = finalMesh;
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.SetTexture("_Texture2D", tex);
        rend.sharedMaterial.SetFloat("_CaveHeight", maxDepth - 1);
        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        PropsPlacer.PlaceObjects(bushPrefabDesert,bushPrefabGreen, tex, 300,transform);

    }

    private Mesh GenerateSurfaceAndCave()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();
        MarchingCubes.MarchAlgorithm(floatGrid, vertices, faces, isoLevel,maxDepth,
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
    private void GenerateBiomesWithShader(ref Texture2D tex)
    {
        BiomeManager biome = new BiomeManager(width, length);
        float[] tempGrid = new float[width * (maxDepth + maxElevation + ADDCEILING) * length];
        ComputeBuffer buffer = new ComputeBuffer(width * (maxDepth + maxElevation + ADDCEILING) * length, sizeof(float));
        buffer.SetData(tempGrid);
        print(buffer.count);
        //Setting variables to compute shader
        shader.SetTexture(0, "biomes", biome.GetTex());
        shader.SetFloat("maxDepth", maxDepth);
        shader.SetFloat("maxElevation", maxElevation);
        shader.SetVector("gridSize", new Vector3(width, (maxDepth + maxElevation + ADDCEILING), length));
        shader.SetBuffer(0, "grid", buffer);
        //Getting information from shader
        shader.Dispatch(0, width / 10, 1, length / 10);
        buffer.GetData(tempGrid);
        buffer.Dispose();
        //translating array from one to three dimensions
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
    /// <summary>
    /// Generate UV for the flat terrain
    /// </summary>
    /// <param name="vert">Vertices of the mesh</param>
    /// <param name="width">Width of the terrain</param>
    /// <param name="length">Length of the terrain</param>
    /// <returns>UV coordinates for each vertex</returns>
    private static Vector2[] BuildSurfaceUV(List<Vector3> vert, float width, float length)
    {
        Vector2[] uv = new Vector2[vert.Count];
        for (int i = 0; i < vert.Count; i++)
        {
            uv[i] = new Vector2(vert[i].x / width * SCALE, vert[i].z / length * SCALE);
        }
        return uv;
    }
    //REMOVE
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(new Vector3(0, maxDepth, 0),Vector3.one);
        Gizmos.DrawCube(new Vector3(0, maxDepth - 50, 0),Vector3.one);
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