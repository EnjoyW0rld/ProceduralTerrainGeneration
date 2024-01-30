using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class SmoothedTerrain : MonoBehaviour
{
    const int SCALE = 1;
    /// <summary>
    /// Extra height for the grid to make sure that high mountains are not being cut
    /// </summary>
    const int ADDCEILING = 10;


    [Header("Terrain dimensions")]
    [SerializeField, Min(1)] private int width;
    [SerializeField, Min(1)] private int length;
    [SerializeField, Min(1)] private int maxElevation, maxDepth;
    [SerializeField, Range(0f, 1f)] private float isoLevel;
    [Header("Shader variables")]
    [SerializeField] private ComputeShader shader;
    [Header("Instancables")]
    [SerializeField] private GameObject bushPrefabDesert;
    [SerializeField] private GameObject bushPrefabGreen;

    private Texture2D tex;
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
        Stopwatch v = new Stopwatch();
        CaveLTree.CreateCave(floatGrid, new Vector3(width / 2, maxDepth - 1, length / 2), 30, 10, -1f, 2);
        CaveLTree.DrawLine(floatGrid, new Vector3(width / 2, maxDepth, length / 2 - 1), new Vector3(width / 2, maxDepth + maxElevation, length / 2));

        Mesh finalMesh = GenerateSurfaceAndCave();

        GetComponent<MeshCollider>().sharedMesh = finalMesh;
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.SetTexture("_Texture2D", tex);
        rend.sharedMaterial.SetFloat("_CaveHeight", maxDepth - 1);
        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        v.print("Seconds to generate -");
        PropsPlacer.PlaceObjects(bushPrefabDesert, bushPrefabGreen, tex, 300, transform);
    }

    private Mesh GenerateSurfaceAndCave()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> faces = new List<int>();
        MarchingCubes.MarchAlgorithm(floatGrid, vertices, faces, isoLevel, maxDepth,
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
        int times = 100;
        BiomeManager biome = new BiomeManager(width, length);
        Vector3 steps = new Vector3(Mathf.Ceil(width / (float)times), 0, Mathf.Ceil(length / (float)times));
        for (int i = 0; i < steps.x; i++)
        {
            for (int l = 0; l < steps.z; l++)
            {
                Vector3 offset = new Vector3(times * i, 0, times * l);
                float[] tempGrid = new float[(times + (int)offset.x) * (maxDepth + maxElevation + ADDCEILING) * (times + (int)offset.z)];
                ComputeBuffer buffer = new ComputeBuffer((times + (int)offset.x) * (maxDepth + maxElevation + ADDCEILING) * (times + (int)offset.z), sizeof(float));
                buffer.SetData(tempGrid);
                shader.SetTexture(0, "biomes", biome.GetTex());
                shader.SetFloat("maxDepth", maxDepth);
                shader.SetFloat("maxElevation", maxElevation);
                shader.SetVector("gridSize", new Vector3(width, (maxDepth + maxElevation + ADDCEILING), length));
                shader.SetVector("offset", offset);
                shader.SetBuffer(0, "grid", buffer);

                shader.Dispatch(0, times / 10, 1, times / 10);
                buffer.GetData(tempGrid);
                buffer.Dispose();
                for (int x = 0; x < times; x++)
                {
                    if (x + offset.x >= width) continue;
                    for (int z = 0; z < times; z++)
                    {
                        if (z + offset.z >= length) continue;
                        for (int y = 0; y < (maxDepth + maxElevation + ADDCEILING); y++)
                        {
                            if (y + i >= maxDepth + maxElevation + ADDCEILING) continue;
                            floatGrid[(x + (int)offset.x), y + i, (z + (int)offset.z)] = tempGrid[(int)((x + offset.x) + times * y + times * (maxDepth + maxElevation + ADDCEILING) * (z))];
                            //floatGrid[(x + (int)offset.x), y, (z + (int)offset.z)] = tempGrid[(x + 100 * i) + 100 * y + 100 * (maxDepth + maxElevation + ADDCEILING) * (z + (int)offset.z)];
                        }
                    }
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