using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for creating caves using L-tree algorithms
/// Writes data directly to float or int grid
/// </summary>
public class CaveLTree : MonoBehaviour
{
    //Int grid DrawLine
    /// <summary>
    /// Apply values to the grid points between two positions
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    public static void DrawLine(int[,,] grid, int[] pos1, int[] pos2, bool fillWithOne = true)
    {
        int fillValue = fillWithOne ? 1 : 0;
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
            SetPointsAround(width, rounded, grid, fillValue);
        }
    }
    public static void DrawLine(int[,,] grid, int[] pos1, int[] pos2, int width, bool fillWithOne = true)
    {
        int fillValue = fillWithOne ? 1 : 0;
        Vector3 vpos1 = new Vector3(pos1[0], pos1[1], pos1[2]);
        Vector3 vpos2 = new Vector3(pos2[0], pos2[1], pos2[2]);
        Vector3 dir = (vpos2 - vpos1).normalized;
        int steps = (int)Vector3.Distance(vpos1, vpos2);
        for (int i = 0; i < steps; i++)
        {
            Vector3Int rounded = new Vector3Int(pos1[0] + Mathf.RoundToInt((dir.x * i)),
                pos1[1] + Mathf.RoundToInt((dir.y * i)),
                pos1[2] + Mathf.RoundToInt((dir.z * i)));
            SetPointsAround(width, rounded, grid, fillValue);
        }
    }

    public static void DrawLine(int[,,] grid, Vector3 pos1, Vector3 pos2, bool fillWithOne = true) => DrawLine(grid, new int[] { (int)pos1.x, (int)pos1.y, (int)pos1.z }, new int[] { (int)pos2.x, (int)pos2.y, (int)pos2.z }, fillWithOne);
    public static void DrawLine(int[,,] grid, Vector3 pos1, Vector3 pos2, int width, bool fillWithOne = true) => DrawLine(grid, new int[] { (int)pos1.x, (int)pos1.y, (int)pos1.z }, new int[] { (int)pos2.x, (int)pos2.y, (int)pos2.z }, width, fillWithOne);
    //Float grid DrawLine
    public static void DrawLine(float[,,] grid, int[] pos1, int[] pos2, int width = 1, bool fillWithOne = true)
    {
        int fillValue = fillWithOne ? 1 : 0;
        Vector3 vpos1 = new Vector3(pos1[0], pos1[1], pos1[2]);
        Vector3 vpos2 = new Vector3(pos2[0], pos2[1], pos2[2]);
        Vector3 dir = (vpos2 - vpos1).normalized;
        int steps = (int)Vector3.Distance(vpos1, vpos2);
        for (int i = 0; i < steps; i++)
        {
            Vector3Int rounded = new Vector3Int(pos1[0] + Mathf.RoundToInt((dir.x * i)),
                pos1[1] + Mathf.RoundToInt((dir.y * i)),
                pos1[2] + Mathf.RoundToInt((dir.z * i)));
            SetPointsAround(width, rounded, grid, fillValue);
        }
    }
    public static void DrawLine(float[,,] grid, Vector3 pos1, Vector3 pos2, int width = 1, bool fillWithOne = true) => DrawLine(grid, new int[] { (int)pos1.x, (int)pos1.y, (int)pos1.z }, new int[] { (int)pos2.x, (int)pos2.y, (int)pos2.z }, width, fillWithOne);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="entryPos"></param>
    /// <param name="maxDist">Maximal distance from one point to another</param>
    /// <param name="repetition">Amount of iteration algorithm will make</param>
    /// <param name="verticalDir">Coefficient determining how deep cave goes</param>
    /// <param name="fillWithOne">true = solid part is filled with value 1, false = filled with value 0</param>
    public static void CreateCave(int[,,] grid, Vector3 entryPos, int maxDist, int repetition, int verticalDir, bool fillWithOne = true)
    {
        LConnection init = new LConnection(entryPos, maxDist, repetition, LConnection.State.A, new Vector3(0, verticalDir, 0).normalized);
        List<LConnection> conArr = init.StartCreation();
        ApplyLTreeToGrid(conArr, grid, fillWithOne);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="entryPos"></param>
    /// <param name="maxDist">Maximal distance from one point to another</param>
    /// <param name="repetition">Amount of iteration algorithm will make</param>
    /// <param name="verticalDir">Coefficient determining how deep cave goes</param>
    /// <param name="fillWithOne">true = solid part is filled with value 1, false = filled with value 0</param>
    public static void CreateCave(float[,,] grid, Vector3 entryPos, int maxDist, int repetition, float verticalDir, int width, bool fillWithOne = true)
    {
        LConnection init = new LConnection(entryPos, maxDist, repetition, LConnection.State.A, new Vector3(0, verticalDir, 0));
        List<LConnection> conArr = init.StartCreation();
        ApplyLTreeToGrid(conArr, grid, width, fillWithOne);
    }

    /// <summary>
    /// Apply value to all adjacent points
    /// </summary>
    /// <param name="width">Radius in which value is applied</param>
    /// <param name="pos"></param>
    /// <param name="grid"></param>
    /// <param name="fillValue"></param>
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
    private static void SetPointsAround(int width, Vector3Int pos, float[,,] grid, int fillValue)
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
                    //grid[x, y, z] = .3f;// fillValue;
                    grid[x, y, z] = fillValue;
                    //grid[x, y, pos.z + 1] = fillValue;
                }
            }
        }
    }

    /// <summary>
    /// Fills in grid with values from generated cave
    /// </summary>
    /// <param name="conns">List of all end points of cave</param>
    /// <param name="grid"></param>
    /// <param name="width"></param>
    /// <param name="fillWithOne"></param>
    private static void ApplyLTreeToGrid(List<LConnection> conns, float[,,] grid, int width, bool fillWithOne = true)
    {
        List<LConnection> built = new List<LConnection>();
        for (int i = 0; i < conns.Count; i++)
        {
            LConnection currentConn = conns[i];
            while (currentConn.previousConnection != null)
            {
                if (built.Contains(currentConn)) break;
                DrawLine(grid, currentConn.currentPos, currentConn.previousConnection.currentPos, width, fillWithOne);
                built.Add(currentConn);
                currentConn = currentConn.previousConnection;
            }
        }
    }
    private static void ApplyLTreeToGrid(List<LConnection> conns, int[,,] grid, bool fillWithOne = true)
    {
        List<LConnection> built = new List<LConnection>();
        for (int i = 0; i < conns.Count; i++)
        {
            LConnection currentConn = conns[i];
            while (currentConn.previousConnection != null)
            {
                if (built.Contains(currentConn)) break;
                DrawLine(grid, currentConn.currentPos, currentConn.previousConnection.currentPos, fillWithOne);
                built.Add(currentConn);
                currentConn = currentConn.previousConnection;
            }
        }
    }
}

/// <summary>
/// Represents singular point for L-tree algorithm
/// </summary>
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
    /// <summary>
    /// Create next point based on the state of current
    /// </summary>
    /// <param name="conn"></param>
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
        Vector3 nextPos = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(1, maxDist), Random.Range(0, maxDist));
        LConnection nextConnection = new LConnection(nextPos, maxDist, repetition - 1, State.B, dir * STRAIGHTENING, this);
        nextConnection.SpawnNext(conn);
    }
    private void SpawnFromA(List<LConnection> conn)
    {
        Vector3 nextPos1 = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(1, maxDist), Random.Range(-maxDist, maxDist));
        Vector3 nextPos2 = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(1, maxDist), Random.Range(-maxDist, maxDist));
        LConnection nextConnection1 = new LConnection(nextPos1, maxDist, repetition - 1, State.B, dir * STRAIGHTENING, this);
        LConnection nextConnection2 = new LConnection(nextPos2, maxDist, repetition - 1, State.A, dir * STRAIGHTENING, this);
        nextConnection1.SpawnNext(conn);
        nextConnection2.SpawnNext(conn);
    }
}