using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PlaneBuilder : MonoBehaviour
{
    

    [SerializeField] private GameObject prefab;

    [SerializeField] private int width, length;
    [SerializeField] private float maxHeight;
    [SerializeField] private int octaves;
    [SerializeField] private float scale;


    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private List<List<Matrix4x4>> matrices;

    private void Start()
    {

        matrices = new List<List<Matrix4x4>>();
        matrices.Add(new List<Matrix4x4>());
        int amount = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {

                // --- uncomment for perlin noise ---
                //float y = Mathf.PerlinNoise(x / (float)width * scale, z / (float)length * scale);
                float y = Fbm.GetValue(x / (float)width, z / (float)length, octaves, scale);

                matrices[matrices.Count - 1].Add(Matrix4x4.TRS(new Vector3(x, Mathf.Lerp(0, maxHeight, y), z), Quaternion.identity, Vector3.one));
                amount++;
                if (amount >= 1022)
                {
                    matrices.Add(new List<Matrix4x4>());
                    amount = 0;
                }
            }
        }

    }
    private void Update()
    {
        for (int i = 0; i < matrices.Count; i++)
        {
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices[i]);
        }
    }

}
