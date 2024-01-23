using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsPlacer : MonoBehaviour
{
    public static void PlaceObjects(GameObject desertPrefab, GameObject greenPrefab, Texture2D tex, int count, Transform parent)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(0, tex.width / 3);
            int z = Random.Range(0, tex.height / 3);
            if (Physics.Raycast(new Vector3(x, 500, z), Vector3.down, out RaycastHit hit))
            {

                GameObject inst = Instantiate(tex.GetPixel(x * 3, z * 3).r < .5f ? desertPrefab : greenPrefab, hit.point, Quaternion.identity, parent);
                inst.transform.up = hit.normal * .1f + inst.transform.up * .9f;
                inst.transform.Rotate(inst.transform.up, Random.Range(0, 360));
            }
        }
    }
    public static void PlaceObjects(GameObject prefab, Texture2D tex, int count, Transform parent)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(0, tex.width / 3);
            int z = Random.Range(0, tex.height / 3);
            if (Physics.Raycast(new Vector3(x, 500, z), Vector3.down, out RaycastHit hit))
            {
                GameObject inst = Instantiate(prefab, hit.point, Quaternion.identity, parent);
                inst.transform.up = hit.normal;

            }
        }
    }
}