using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TriengulationNew : MonoBehaviour
{
    [SerializeField] private Vector3[] points = new Vector3[3];
    private Vector3 GetMidPointBetween(Vector3[] points)
    {
        if (points.Length != 2)
        {
            Debug.LogError("Triyng to get middle point for not 2 points");
            return Vector3.zero;
        }
        return (points[1] - points[0]) / 2 + points[0];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(points[0], .5f);
        Gizmos.DrawSphere(points[1], .7f);
        Gizmos.DrawSphere(points[2], .9f);


        for (int i = 0; i < points.Length; i++)
        {
            //Gizmos.DrawSphere(points[i], .5f);
        }
        Gizmos.color = Color.green;
        Vector3 abm = GetMidPointBetween(new Vector3[] { points[0], points[1] });
        Vector3 acm = GetMidPointBetween(new Vector3[] { points[0], points[2] });
        Gizmos.DrawSphere(abm, .5f);
        Gizmos.DrawSphere(acm, .5f);

        Gizmos.color = Color.blue;
        Vector3 midDist = abm - acm;
        Gizmos.DrawLine(acm, acm + midDist);
        Gizmos.DrawWireSphere(acm + midDist, .2f);

        Gizmos.color = Color.yellow;
        Vector3 acmNormal = (points[2] - points[0]).normalized;
        acmNormal = new Vector3(-acmNormal.z, 0, acmNormal.x);
        Gizmos.DrawLine(acm, acm + acmNormal);
        Gizmos.DrawWireSphere(acm + acmNormal, .2f);


        Vector3 abNormal = (points[1] - points[0]).normalized;
        Gizmos.DrawLine(abm, abm + abNormal);
        Gizmos.DrawWireSphere(abm + abNormal, .2f);

        Vector3 abmNormal = new Vector3(-abNormal.z,0,abNormal.x);

        float dist = Vector3.Dot(midDist, abNormal);
        Gizmos.DrawWireSphere(acm + acmNormal * dist,.5f);

        Plane newPlane = new Plane(abNormal, abm);
        Ray r = new Ray(acm, acmNormal);
        newPlane.Raycast(r,out float enter);
        Vector3 rightPos = r.GetPoint(enter);
        //Gizmos.DrawWireSphere(rightPos,1f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(acm - (acmNormal*20), acm + acmNormal * 20);
        Gizmos.DrawLine(abm - abmNormal * 20, abm + abmNormal * 20);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(rightPos, Vector3.Distance(rightPos, points[0]));

    }
}