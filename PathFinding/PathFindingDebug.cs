using UnityEngine;

public class PathFindingDebug : MonoBehaviour
{
    public PathResult Result
    {
        set 
        {
            requestStart = value.request.start;
            requestGridSize = value.request.gridSize;
            requestRes = value.request.resolution;
            requestInformer = value.request.informer;
            requesResult = value.Path;
        }
    }


    public bool drawGrid = true;
    public bool drawResultWaypoints = true;




    /////////////////////////////////////////
    // Gizmos
    /////////////////////////////////////////
    private Vector3Int requestStart;
    private int requestGridSize = 1;
    private float requestRes = 1;
    private Vector3[] requesResult = null;
    private TerrainInformer requestInformer = null;
    private Vector3 LocalToGlobalPos(Vector3Int start, Vector2Int localPos)
    {
        Vector3Int gridOrigin = start;
        gridOrigin.x -= requestGridSize / 2;
        gridOrigin.z -= requestGridSize / 2;

        Vector3 globalPos = gridOrigin;
        globalPos.x += localPos.x;
        globalPos.z += localPos.y;
        globalPos /= requestRes;
        return globalPos;
    }

    private void DrawGridTraversable()
    {
        if (requesResult == null || requestInformer == null) return;
        for (int x = 0; x < requestGridSize; ++x)
        {
            for (int z = 0; z < requestGridSize; ++z)
            {
                Vector3 pos = LocalToGlobalPos(requestStart, new Vector2Int(x, z));

                if (requestInformer.IsTraversable(pos))
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(pos, Vector3.one * 0.1f);
            }
        }
    }
    private void DrawResultPath()
    {
        if (requesResult == null) return;
        Gizmos.color = Color.red;
        foreach (Vector3 waypoint in requesResult)
        {
            Gizmos.DrawCube(waypoint, Vector3.one * 0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (drawGrid) DrawGridTraversable();
        
        if (drawResultWaypoints) DrawResultPath();
    }
}