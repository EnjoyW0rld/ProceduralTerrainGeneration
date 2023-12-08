using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothedTerrain : MonoBehaviour
{
    [SerializeField] private int width, length, maxElevation, maxDepth;
    [SerializeField, Range(0f, 1f)] private float isoLevel;
    private float[,,] floatGrid;
    // Start is called before the first frame update
    void Start()
    {
        floatGrid = new float[width, maxDepth + maxElevation, length];
        Texture2D tex = new Texture2D(width, length);
        GenerateBiomesWithNoiseSmoothed(floatGrid, width, length, maxElevation, maxDepth, tex);
        //Generating cave
        int[,,] grid = new int[width, maxDepth + maxElevation, length];
        CaveLTree.CreateCave(grid, new Vector3(width / 2, maxDepth + 2, length / 2), 10, 10, -2);
        Mesh caveMesh = MarchingCubes.GetMeshMarchingCubes(grid);

        //Surface creation
        Mesh mesh = GetSurcafeMesh();
        
        //combining meshes
        CombineInstance[] meshes = CombineMeshes(mesh, caveMesh);

        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes(meshes);

        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        //print(MarchingCubes.GetPos(new Vector3(1, 14, 1), new Vector3(1, 14, 2), 0.2f, 0.3f, 0.25f));
    }

    private Mesh GetSurcafeMesh()
    {
        Mesh mesh = new Mesh();
        MeshData data = MarchingCubes.GetMeshMarchingCubesData(floatGrid, isoLevel);
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.faces.ToArray();
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
    private static void GenerateBiomesWithNoiseSmoothed(float[,,] grid, int width, int length, int maxElevation, int maxDepth, Texture2D tex = null)
    {
        BiomeManager biome = new BiomeManager(width, length);
        //int[,,] places = new int[width, maxDepth + maxElevation, length];
        float randVal = System.DateTime.Now.Second;
        float scaleVal = 10f;
        //Texture2D tex = new Texture2D(width, length); //Generating texture

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
                    if (tex != null) tex.SetPixel(x, z, new Color(0, 0, 0));
                }
                else
                {
                    noiseValue = Fbm.GetValue(realX, realZ, 6, 1);
                    if (tex != null) tex.SetPixel(x, z, new Color(1, 1, 1));

                }
                for (int y = maxDepth; y < maxElevation + maxDepth; y++)
                {
                    if (y == maxDepth + maxElevation - 1)
                    {
                        grid[x, y, z] = 1;
                        continue;
                    }
                    //(y-maxDepth) (maxElevation - 2)
                    grid[x, y, z] = Mathf.Clamp01(((y - maxDepth) / ((float)maxElevation - 2) + noiseValue) / 2f);
                    //grid[x, y, z] = ((y - maxDepth) / (float)maxElevation + noiseValue) / 2f;


                    //grid[x, y, z] = y / (float)(maxElevation + maxDepth) + noiseValue;
                    //print(y / (float)(maxElevation + maxDepth) - noiseValue);
                }
                /*int mappedHeight = (int)Mathf.Lerp(maxDepth, maxElevation + maxDepth, noiseValue);
                //tex.SetPixel(x, z, new Color(biomeNum / 10f, biomeNum / 10f, biomeNum / 10f));
                for (int y = mappedHeight; y > maxDepth; y--)
                {
                    //grid[x, y, z] = 1;
                    grid[x, y, z] = y / (float)mappedHeight;//Mathf.Lerp(1, 0, y / mappedHeight);
                    print(y / (float)mappedHeight);
                }
                grid[x, mappedHeight, z] = 1;*/
            }

        }
        tex.Apply();
    }
    private void OnDrawGizmosSelected()
    {
        if (floatGrid == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int y = maxDepth - 1; y < floatGrid.GetLength(1); y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (floatGrid[x, y, z] > isoLevel) continue;
                    Gizmos.color = new Color(floatGrid[x, y, z], floatGrid[x, y, z], floatGrid[x, y, z]);
                    Gizmos.DrawCube(new Vector3(x, y, z), new Vector3(.2f, .2f, .2f));
                }
            }
        }
    }
}
