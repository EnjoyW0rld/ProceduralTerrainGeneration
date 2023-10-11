using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation : MonoBehaviour
{
    [SerializeField] private Vector3[] oldPoints = new Vector3[3];
    [SerializeField] private int pointsCount = 3;
    private Vector3[] triPoints;
    private List<Vector3[]> triangles;

    private void Start()
    {
        triangles = new List<Vector3[]>();
        triPoints = new Vector3[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            triPoints[i] = new Vector3(Random.Range(0, 60f), 0, Random.Range(0, 60f));
        }
        for (int x = 0; x < triPoints.Length; x++)
        {
            for (int y = x; y < triPoints.Length; y++)
            {
                for (int z = y; z < triPoints.Length; z++)
                {
                    if (!IsOverlaping(new int[] { x, y, z }))
                        triangles.Add(new Vector3[] { triPoints[x], triPoints[y], triPoints[z] });
                }
            }
        }
    }

    private bool IsOverlaping(int[] indecies)
    {
        Vector3[] currentTries = new Vector3[] { triPoints[indecies[0]], triPoints[indecies[1]], triPoints[indecies[2]] };
        Vector3 midPoint = GetMiddlePoint(currentTries);
        float radius = Vector3.Distance(midPoint, triPoints[indecies[0]]);//GetRadius(length.x, length.y, length.z);
        
        for (int i = 0; i < triPoints.Length; i++)
        {
            float dist = Vector3.Distance(midPoint, triPoints[i]);
            print("dist " + dist);
            if (dist < radius)
            {
                if (i != indecies[0] && i != indecies[1] && i != indecies[2]) return true;
            }
        }
        return false;
    }
    private float GetRadius(float a, float b, float c)
    {
        float p = a + b + c;
        float abc = a * b * c;
        return abc / (4 * Mathf.Sqrt(p * (p - a) * (p - b) * (p - c)));
    }
    private Vector3 GetMidPointBetween(Vector3[] points)
    {
        if (points.Length != 2)
        {
            Debug.LogError("Triyng to get middle point for not 2 points");
            return Vector3.zero;
        }
        return (points[1] - points[0]) / 2 + points[0];
    }

    private Vector3 GetMiddlePoint(Vector3[] points)
    {
        Vector3 abm = GetMidPointBetween(new Vector3[] { points[1], points[0] });//(points[1] - points[0]) / 2 + points[0];
        Vector3 acm = GetMidPointBetween(new Vector3[] { points[2], points[0] });

        Vector3 abNormal = (points[1] - points[0]).normalized;
        Vector3 acmNormal = (points[2] - points[0]).normalized;
        acmNormal = new Vector3(-acmNormal.z, 0, acmNormal.x);


        Plane newPlane = new Plane(abNormal, abm);
        Ray r = new Ray(acm, acmNormal);
        newPlane.Raycast(r, out float enter);
        Vector3 rightPos = r.GetPoint(enter);

        return rightPos;
    }

    private Vector3 GetLength(Vector3[] points)
    {
        if (points.Length != 3) return Vector3.zero;

        Vector3 res = Vector3.zero;
        res.x = Vector3.Distance(points[0], points[1]);
        res.y = Vector3.Distance(points[0], points[2]);
        res.z = Vector3.Distance(points[1], points[2]);
        return res;
    }
    private void OnDrawGizmos()
    {
        if (triPoints != null)
        {
            if (triangles != null)
            {
                for (int i = 0; i < triangles.Count; i++)
                {
                    Gizmos.DrawLine(triangles[i][0], triangles[i][1]);
                    Gizmos.DrawLine(triangles[i][0], triangles[i][2]);
                    Gizmos.DrawLine(triangles[i][1], triangles[i][2]);
                }
            }
            for (int i = 0; i < triPoints.Length; i++)
            {
                Gizmos.DrawWireSphere(triPoints[i], .5f);
            }
        }
    }
    /**
    private void OnDrawGizmos()
    {
        if (points == null || points.Length == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], .5f);
        }
        Vector3 middlePoint = GetMiddlePoint(new Vector3[] { points[0], points[1], points[2] });
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(middlePoint, Vector3.Distance(middlePoint, points[0]));
        return;
    }
    /**/
}
