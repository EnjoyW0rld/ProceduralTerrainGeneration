using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int maxElevation;
    [SerializeField] private int maxDepth;
    [SerializeField] private int size;
    //Debug variables
    [SerializeField] private Texture2D tex;

    private int[,,] grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = new int[size, maxDepth + maxElevation, size];
        tex = new Texture2D(size, size);

        GenerateBiomesWithNoises(grid, size, size, maxElevation, maxDepth, tex);
        FillWithOne(grid, maxElevation);
        CaveLTree.CreateCave(grid, new Vector3(size / 2, maxDepth + 2, size / 2), 10, 10, -2, false);
        CaveLTree.DrawLine(grid, new Vector3(size / 2, maxDepth + 2, size / 2), new Vector3(size / 2, maxDepth + maxElevation, size / 2), false);

        tex = new LinearBlur().Blur(tex, 2, 2);
        //Creating Mesh
        MeshData meshData = MarchingCubes.GetDataMarchingCubes(grid);
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //Applying data to mesh
        mesh.SetVertices(meshData.vertices);
        mesh.SetTriangles(meshData.faces, 0);
        mesh.uv = BuildSurfaceUV(meshData.vertices, size, size);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial.SetTexture("_Texture2D", tex);
        rend.sharedMaterial.SetFloat("_CaveHeight", maxDepth);

    }

    private static void FillWithOne(int[,,] grid, int fillHeight)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1) - fillHeight; y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    grid[x, y, z] = 1;
                }
            }
        }
    }
    private static void GenerateBiomesWithNoises(int[,,] grid, int width, int length, int maxElevation, int maxDepth, Texture2D tex = null)
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
                int mappedHeight = (int)Mathf.Lerp(maxDepth, maxElevation + maxDepth, noiseValue);
                //tex.SetPixel(x, z, new Color(biomeNum / 10f, biomeNum / 10f, biomeNum / 10f));
                for (int y = mappedHeight; y > maxDepth; y--)
                {
                    grid[x, y, z] = 1;
                }
                grid[x, mappedHeight, z] = 1;
            }

        }
        tex.Apply();
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
                int mappedHeight = (int)Mathf.Lerp(maxDepth, maxElevation + maxDepth, noiseValue);
                //tex.SetPixel(x, z, new Color(biomeNum / 10f, biomeNum / 10f, biomeNum / 10f));
                for (int y = mappedHeight; y > maxDepth; y--)
                {
                    //grid[x, y, z] = 1;
                    grid[x, y, z] = y / mappedHeight;//Mathf.Lerp(1, 0, y / mappedHeight);
                }
                grid[x, mappedHeight, z] = 1;
            }

        }
        tex.Apply();
    }
    private static void InvertGrid(int[,,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    grid[x, y, z] = grid[x, y, z] == 1 ? 0 : 1;
                }
            }
        }
    }
    private static void CopyToArray(int[,,] from, int[,,] to)
    {
        if (from.GetLength(0) > to.GetLength(0) || from.GetLength(1) > to.GetLength(1) || from.GetLength(2) > to.GetLength(2))
        {
            Debug.LogError("Trying to copy to the smaller array");
            return;
        }
        for (int x = 0; x < from.GetLength(0); x++)
        {
            for (int y = 0; y < from.GetLength(1); y++)
            {
                for (int z = 0; z < from.GetLength(2); z++)
                {
                    to[x, y, z] = from[x, y, z];
                }
            }
        }
    }
    private static Vector2[] BuildSurfaceUV(List<Vector3> vert, float width, float length)
    {
        Vector2[] uv = new Vector2[vert.Count];
        for (int i = 0; i < vert.Count; i++)
        {
            uv[i] = new Vector2(vert[i].x / width, vert[i].z / length);
        }
        return uv;
    }
}
class LinearBlur
{
    private float _rSum = 0;
    private float _gSum = 0;
    private float _bSum = 0;

    private Texture2D _sourceImage;
    private int _sourceWidth;
    private int _sourceHeight;
    private int _windowSize;

    public Texture2D Blur(Texture2D image, int radius, int iterations)
    {
        _windowSize = radius * 2 + 1;
        _sourceWidth = image.width;
        _sourceHeight = image.height;

        var tex = image;

        for (var i = 0; i < iterations; i++)
        {
            tex = OneDimensialBlur(tex, radius, true);
            tex = OneDimensialBlur(tex, radius, false);
        }

        return tex;
    }

    private Texture2D OneDimensialBlur(Texture2D image, int radius, bool horizontal)
    {
        _sourceImage = image;

        var blurred = new Texture2D(image.width, image.height, image.format, false);

        if (horizontal)
        {
            for (int imgY = 0; imgY < _sourceHeight; ++imgY)
            {
                ResetSum();

                for (int imgX = 0; imgX < _sourceWidth; imgX++)
                {
                    if (imgX == 0)
                        for (int x = radius * -1; x <= radius; ++x)
                            AddPixel(GetPixelWithXCheck(x, imgY));
                    else
                    {
                        var toExclude = GetPixelWithXCheck(imgX - radius - 1, imgY);
                        var toInclude = GetPixelWithXCheck(imgX + radius, imgY);

                        SubstPixel(toExclude);
                        AddPixel(toInclude);
                    }

                    blurred.SetPixel(imgX, imgY, CalcPixelFromSum());
                }
            }
        }

        else
        {
            for (int imgX = 0; imgX < _sourceWidth; imgX++)
            {
                ResetSum();

                for (int imgY = 0; imgY < _sourceHeight; ++imgY)
                {
                    if (imgY == 0)
                        for (int y = radius * -1; y <= radius; ++y)
                            AddPixel(GetPixelWithYCheck(imgX, y));
                    else
                    {
                        var toExclude = GetPixelWithYCheck(imgX, imgY - radius - 1);
                        var toInclude = GetPixelWithYCheck(imgX, imgY + radius);

                        SubstPixel(toExclude);
                        AddPixel(toInclude);
                    }

                    blurred.SetPixel(imgX, imgY, CalcPixelFromSum());
                }
            }
        }

        blurred.Apply();
        return blurred;
    }

    private Color GetPixelWithXCheck(int x, int y)
    {
        if (x <= 0) return _sourceImage.GetPixel(0, y);
        if (x >= _sourceWidth) return _sourceImage.GetPixel(_sourceWidth - 1, y);
        return _sourceImage.GetPixel(x, y);
    }

    private Color GetPixelWithYCheck(int x, int y)
    {
        if (y <= 0) return _sourceImage.GetPixel(x, 0);
        if (y >= _sourceHeight) return _sourceImage.GetPixel(x, _sourceHeight - 1);
        return _sourceImage.GetPixel(x, y);
    }

    private void AddPixel(Color pixel)
    {
        _rSum += pixel.r;
        _gSum += pixel.g;
        _bSum += pixel.b;
    }

    private void SubstPixel(Color pixel)
    {
        _rSum -= pixel.r;
        _gSum -= pixel.g;
        _bSum -= pixel.b;
    }

    private void ResetSum()
    {
        _rSum = 0.0f;
        _gSum = 0.0f;
        _bSum = 0.0f;
    }

    Color CalcPixelFromSum()
    {
        return new Color(_rSum / _windowSize, _gSum / _windowSize, _bSum / _windowSize);
    }
}