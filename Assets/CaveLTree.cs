using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CaveLTree : MonoBehaviour
{
    private int[,,] grid;
    [SerializeField] private int[] poss;
    [SerializeField] private int maxDist;


    [Header("Cellular Automata")]
    [SerializeField] private float chanceToSpawnAlive = .45f;
    [SerializeField] private int deathLimit = 3;
    [SerializeField] private int birthLimit = 4;
    private MeshFilter filter;
    private List<LConnection> conns;

    //DEBUG VARIABLES
    private Vector3 startPos;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        grid = new int[60, 40, 60];
        LConnection conn = new LConnection(new Vector3(30, 30, 30), maxDist, 6, LConnection.State.A, new Vector3(0, -1, 0).normalized);
        startPos = new Vector3(30, 30, 30);
        conns = conn.StartCreation();
        //LConnection iter = conns[0];
        ApplyLTreeToGrid(conns, grid);

        GetComponent<MeshFilter>().mesh = MarchingCubes.GetMeshMarchingCubes(grid);
    }
    private void ApplyLTreeToGrid(List<LConnection> conns, int[,,] grid)
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
    private void DrawLine(int[,,] grid, int[] pos1, int[] pos2)
    {
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
            SetPointsAround(width, rounded, grid);
            //grid[pos1[0] + Mathf.RoundToInt((dir.x * i)), pos1[1] + Mathf.RoundToInt((dir.y * i)), pos1[2] + Mathf.RoundToInt((dir.z * i))] = 1;
        }
    }
    private void DrawLine(int[,,] grid, Vector3 pos1, Vector3 pos2) => DrawLine(grid, new int[] { (int)pos1.x, (int)pos1.y, (int)pos1.z }, new int[] { (int)pos2.x, (int)pos2.y, (int)pos2.z });
    /// <summary>
    /// Helper function for DrawLine func
    /// </summary>
    /// <param name="width"></param>
    /// <param name="pos"></param>
    /// <param name="grid"></param>
    private void SetPointsAround(int width, Vector3Int pos, int[,,] grid)
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
                    grid[x, y, z] = 1;
                }
            }
        }
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

    #region LegacyFunctions(obsolete)

    IEnumerator IDoSimulation()
    {
        for (int i = 0; i < 15; i++)
        {
            grid = DoSimulationStep(grid);
            filter.mesh = MarchingCubes.GetMeshMarchingCubes(grid);
            print(i);
            yield return new WaitForSeconds(1.5f);
        }
    }
    private int[,,] DoSimulationStep(int[,,] grid)
    {
        int[,,] newGrid = new int[grid.GetLength(0), grid.GetLength(1), grid.GetLength(2)];
        //newGrid = grid.Clone();
        //grid.CopyTo(newGrid, 0);


        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    int nbs = CountAliveNeighbours(grid, x, y, z);

                    if (grid[x, y, z] == 1)
                    {
                        if (nbs < deathLimit) newGrid[x, y, z] = 0;
                        else newGrid[x, y, z] = 1;
                    }
                    else
                    {
                        if (nbs < birthLimit) newGrid[x, y, z] = 1;
                        else newGrid[x, y, z] = 0;
                    }
                }
            }
        }
        return newGrid;
    }
    private int CountAliveNeighbours(int[,,] grid, int x, int y, int z)
    {
        int res = 0;

        for (int px = x - 1; px < x + 2; px++)
        {
            if (px < 0 || px > grid.GetLength(0) - 1)
            {
                res++;
                continue;
            }
            for (int py = y - 1; py < y + 2; py++)
            {
                if (py < 0 || py > grid.GetLength(1) - 1)
                {
                    res++;
                    continue;
                }
                for (int pz = z - 1; pz < z + 2; pz++)
                {
                    if (pz < 0 || pz > grid.GetLength(2) - 1)
                    {
                        res++;
                        continue;
                    }
                    if (px == x && py == y && pz == z) continue;
                    if (grid[px, py, pz] == 1) res++;
                }
            }
        }
        return res;
    }

    #endregion
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
        Vector3 nextPos = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y * Random.Range(-maxDist, maxDist), Random.Range(0, maxDist));
        LConnection nextConnection = new LConnection(nextPos, maxDist, repetition - 1, State.B, dir * STRAIGHTENING, this);
        nextConnection.SpawnNext(conn);
    }
    private void SpawnFromA(List<LConnection> conn)
    {
        //Vector3 nextPos1 = Quaternion.Euler(Random.Range(-45, 45) * 0, Random.Range(-45, 45), Random.Range(-45, 45) * 0) * dir;
        //Vector3 nextPos2 = Quaternion.Euler(Random.Range(-45, 45) * 0, Random.Range(-45, 45), Random.Range(-45, 45) * 0) * dir;
        Vector3 nextPos1 = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y, Random.Range(-maxDist, maxDist));
        Vector3 nextPos2 = currentPos + new Vector3(Random.Range(-maxDist, maxDist), dir.y, Random.Range(-maxDist, maxDist));
        LConnection nextConnection1 = new LConnection(nextPos1, maxDist, repetition - 1, State.B, dir * STRAIGHTENING, this);
        LConnection nextConnection2 = new LConnection(nextPos2, maxDist, repetition - 1, State.A, dir * STRAIGHTENING, this);
        nextConnection1.SpawnNext(conn);
        nextConnection2.SpawnNext(conn);
    }
}