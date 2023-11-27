using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CaveLTree : MonoBehaviour
{
    private int[,,] grid;
    [SerializeField] private int[] poss;
    [SerializeField] private int maxDist;


    private MeshFilter filter;
    private List<LConnection> conns;

    //DEBUG VARIABLES
    private Vector3 startPos;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        grid = new int[60, 40, 60];
        LConnection conn = new LConnection(new Vector3(30, 30, 30), maxDist, 6, LConnection.State.A, new Vector3(0, -5, 0).normalized);
        startPos = new Vector3(30, 30, 30);
        conns = conn.StartCreation();
        //LConnection iter = conns[0];
        ApplyLTreeToGrid(conns, grid);

        GetComponent<MeshFilter>().mesh = MarchingCubes.GetMeshMarchingCubes(grid);
    }
    private static void ApplyLTreeToGrid(List<LConnection> conns, int[,,] grid)
    {
        List<LConnection> built = new List<LConnection>();
        for (int i = 0; i < conns.Count; i++)
        {
            LConnection currentConn = conns[i];
            while (currentConn.previousConnection != null)
            {
                if (built.Contains(currentConn)) break;
                DrawLine(grid, currentConn.currentPos, currentConn.previousConnection.currentPos);
                built.Add(currentConn);
                currentConn = currentConn.previousConnection;
            }
        }
    }

    /// <summary>
    /// Add points to grid to form a line between two points
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    public static void DrawLine(int[,,] grid, int[] pos1, int[] pos2, bool fillWithOne = true)
    {
        int fillValue = fillWithOne ? 1 : 0;
        //grid[pos1[0], pos1[1], pos1[2]] = 1;
        //grid[pos2[0], pos2[1], pos2[2]] = 1;
        Vector3 vpos1 = new Vector3(pos1[0], pos1[1], pos1[2]);
        Vector3 vpos2 = new Vector3(pos2[0], pos2[1], pos2[2]);
        Vector3 dir = (vpos2 - vpos1).normalized;
        int steps = (int)Vector3.Distance(vpos1, vpos2);
        int width = 1;
        for (int i = 0; i < steps; i++)
        {
            Vector3Int rounded = new Vector3Int(pos1[0] + Mathf.RoundToInt((dir.x * i)),
                pos1[1] + Mathf.RoundToInt((dir.y * i)),
                pos1[2] + Mathf.RoundToInt((dir.z * i)));
            SetPointsAround(width, rounded, grid,fillValue);
            //grid[pos1[0] + Mathf.RoundToInt((dir.x * i)), pos1[1] + Mathf.RoundToInt((dir.y * i)), pos1[2] + Mathf.RoundToInt((dir.z * i))] = 1;
        }
    }
    public static void DrawLine(int[,,] grid, Vector3 pos1, Vector3 pos2,bool fillWithOne = true) => DrawLine(grid, new int[] { (int)pos1.x, (int)pos1.y, (int)pos1.z }, new int[] { (int)pos2.x, (int)pos2.y, (int)pos2.z },fillWithOne);
    /// <summary>
    /// Helper function for DrawLine func
    /// </summary>
    /// <param name="width"></param>
    /// <param name="pos"></param>
    /// <param name="grid"></param>
    private static void SetPointsAround(int width, Vector3Int pos, int[,,] grid, int fillValue)
    {
        for (int x = pos.x - width; x < pos.x + width + 1; x++)
        {
            if (x < 0 || x > grid.GetLength(0) - 1) continue;
            for (int y = pos.y - width; y < pos.y + width + 1; y++)
            {
                if (y < 0 || y > grid.GetLength(1) - 1) continue;
                for (int z = pos.z - width; z < pos.z + width + 1; z++)
                {
                    if (z < 0 || z > grid.GetLength(2) - 1) continue;
                    grid[x, y, z] = fillValue;
                }
            }
        }
    }

    public static void CreateCave(int[,,] grid, Vector3 entryPos, int maxDist, int repetition, int vertialDir)
    {
        LConnection init = new LConnection(entryPos, maxDist, repetition, LConnection.State.A, new Vector3(0, vertialDir, 0).normalized);
        List<LConnection> conArr = init.StartCreation();
        ApplyLTreeToGrid(conArr, grid);
    }

    //DEBUG
    private void DrawConnect(LConnection conn)
    {
        Gizmos.DrawSphere(conn.currentPos, .1f);
        if (conn.previousConnection != null)
        {
            Gizmos.DrawLine(conn.currentPos, conn.previousConnection.currentPos);
            DrawConnect(conn.previousConnection);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (conns != null)
        {
            foreach (var conn in conns)
            {
                DrawConnect((LConnection)conn);
                //Gizmos.DrawSphere(conn.currentPos, .1f);
                //Gizmos.DrawSphere(conn.previousConnection.currentPos, .1f);
                //Gizmos.DrawLine(conn.currentPos, conn.previousConnection.currentPos);
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startPos, .7f);
        }
    }

}

public class LConnection
{
    public enum State { A, B }
    private State state;
    public Vector3 currentPos;
    public LConnection previousConnection;
    private float maxDist;
    private Vector3 dir;
    private const float STRAIGHTENING = .9f;

    private int repetition;
    public LConnection(Vector3 currentPos, float maxDist, int repetition, State state,
        Vector3 dir, LConnection previousConnection = null)
    {
        this.currentPos = currentPos;
        this.maxDist = maxDist;
        this.previousConnection = previousConnection;
        this.repetition = repetition;
        this.state = state;
        this.dir = dir;
    }
    public List<LConnection> StartCreation()
    {
        List<LConnection> conn = new List<LConnection>();
        SpawnNext(conn);
        return conn;
    }
    private void SpawnNext(List<LConnection> conn)
    {
        if (repetition == 0)
        {
            conn.Add(this);
            return;
        }
        switch (state)
        {
            case State.A:
                SpawnFromA(conn);
                break;
            case State.B:
                SpawnFromB(conn);
                break;
        };
    }
    private void SpawnFromB(List<LConnection> conn)
    {
        //Vector3 nextPos = Quaternion.Euler(Random.Range(-45, 45) * 0, Random.Range(-45, 45), Random.Range(-45, 45) * 0) * dir;
        //Debug.Log(nextPos + "next pos");
        Vector3 nextPos = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(1, maxDist), Random.Range(0, maxDist));
        LConnection nextConnection = new LConnection(nextPos, maxDist, repetition - 1, State.B, dir * STRAIGHTENING, this);
        nextConnection.SpawnNext(conn);
    }
    private void SpawnFromA(List<LConnection> conn)
    {
        //Vector3 nextPos1 = Quaternion.Euler(Random.Range(-45, 45) * 0, Random.Range(-45, 45), Random.Range(-45, 45) * 0) * dir;
        //Vector3 nextPos2 = Quaternion.Euler(Random.Range(-45, 45) * 0, Random.Range(-45, 45), Random.Range(-45, 45) * 0) * dir;
        Vector3 nextPos1 = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(1, maxDist), Random.Range(-maxDist, maxDist));
        Vector3 nextPos2 = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(1, maxDist), Random.Range(-maxDist, maxDist));
        LConnection nextConnection1 = new LConnection(nextPos1, maxDist, repetition - 1, State.B, dir * STRAIGHTENING, this);
        LConnection nextConnection2 = new LConnection(nextPos2, maxDist, repetition - 1, State.A, dir * STRAIGHTENING, this);
        nextConnection1.SpawnNext(conn);
        nextConnection2.SpawnNext(conn);
    }
}