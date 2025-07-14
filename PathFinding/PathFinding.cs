using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TerrainInformer
{
    bool IsTraversable(Vector3 position);
}

public class PathFinding
{
    private class Node : IHeapItem<Node>
    {
        public Node parent;
        public readonly bool traversable;
        public float gCost, hCost;
        public readonly Vector2Int localPos;

        public float fCost
        {
            get { return gCost + hCost; }
        }
        public int HeapIndex { get; set; }

        public Node(bool traversable, Vector2Int localPos)
        {
            this.traversable = traversable;
            this.localPos = localPos;
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0)
                compare = hCost.CompareTo(nodeToCompare.hCost);
            return -compare;
        }
    }



    public static PathResult FindPath(PathRequest request)
    {
        // trivial case: already at target
        if (request.start == request.target) 
            return new PathResult(Array.Empty<Vector3>(), true, request);


        Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
        Heap<Node> openSet = new Heap<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        Node[] neighbours = new Node[8];

        Vector2Int startLocalPos = Vector2Int.zero;
        startLocalPos.x = request.gridSize / 2;
        startLocalPos.y = request.gridSize / 2;
        Node startNode = new Node(true, startLocalPos);

        Vector2Int targetLocalPos = Vector2Int.zero;
        targetLocalPos.x = request.target.x - request.start.x;
        targetLocalPos.y = request.target.z - request.start.z;
        targetLocalPos += startLocalPos;
        Vector2Int targetLocalPosRaw = targetLocalPos;

        // clamp end within grid
        targetLocalPos.x = Mathf.Clamp(targetLocalPos.x, 0, request.gridSize-1);
        targetLocalPos.y = Mathf.Clamp(targetLocalPos.y, 0, request.gridSize-1);
        Node targetNode = new Node(true, targetLocalPos);

        startNode.hCost = GetDistance(startNode, targetNode);
        grid.Add(startNode.localPos, startNode);
        grid.Add(targetNode.localPos, targetNode);
        openSet.Add(startNode);

        Node closestNode = startNode;
        while (openSet.Count > 0)
        {
            if (request.Cancelled)
                return null;

            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                // path found
                return new PathResult(SimplifyPath(request, RetracePath(startNode, currentNode)), 
                    currentNode.localPos == targetLocalPosRaw, 
                    request);
            }

            if (currentNode.hCost < closestNode.hCost)
                closestNode = currentNode;

            // updates "neighbours" array with neighbour nodes
            GetNeighbours(request, grid, currentNode, neighbours);

            foreach (Node neighbour in neighbours)
            {
                if (neighbour == null || !neighbour.traversable || closedSet.Contains(neighbour))
                    continue;

                float newMoveCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMoveCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // did not find path, return closest path
        return new PathResult(SimplifyPath(request, RetracePath(startNode, closestNode)), false, request);
    }

    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        return path;
    }

    private static Vector3[] SimplifyPath(PathRequest request, List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = path.Count - 1; i >= 1; --i)
        {
            Vector2 directionNew = path[i - 1].localPos - path[i].localPos;
            if (directionNew != directionOld || i <= 1)
            {
                waypoints.Add(LocalToGlobalPos(request, path[i].localPos));
            }
            directionOld = directionNew;
        }
        waypoints.Add(LocalToGlobalPos(request, path[0].localPos));
        return waypoints.ToArray();
    }

    private static float GetDistance(Node a, Node b)
    {
        // squared distance is faster and equivilant
        // to squared-root distance for comparison sake
        return Vector2.SqrMagnitude(a.localPos - b.localPos);
    }

    private static void GetNeighbours(PathRequest request, Dictionary<Vector2Int, Node> grid, Node center, Node[] neighbours)
    {
        Vector2Int localPos = Vector2Int.zero;
        Vector3 pos = Vector3.zero;
        Node node = null;
        bool upTraversable = false;
        bool rightTraversable = false;
        bool downTraversable = false;
        bool leftTraversable = false;

        // up: 0, 1
        localPos = center.localPos + new Vector2Int(0, 1);
        if (localPos.y < request.gridSize)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                upTraversable = request.informer.IsTraversable(pos);
                node = new Node(upTraversable, localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[0] = node;

        // right: 1, 0
        node = null;
        localPos = center.localPos + new Vector2Int(1, 0);
        if (localPos.x < request.gridSize)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                rightTraversable = request.informer.IsTraversable(pos);
                node = new Node(rightTraversable, localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[1] = node;

        // down: 0, -1
        node = null;
        localPos = center.localPos + new Vector2Int(0, -1);
        if (localPos.y >= 0)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                downTraversable = request.informer.IsTraversable(pos);
                node = new Node(downTraversable, localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[2] = node;

        // left: -1, 0
        node = null;
        localPos = center.localPos + new Vector2Int(-1, 0);
        if (localPos.x >= 0)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                leftTraversable = request.informer.IsTraversable(pos);
                node = new Node(leftTraversable, localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[3] = node;

        // up right: 1, 1
        node = null;
        localPos = center.localPos + new Vector2Int(1, 1);
        if (localPos.x < request.gridSize && upTraversable && rightTraversable)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                node = new Node(request.informer.IsTraversable(pos), localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[4] = node;

        // down right: 1, -1
        node = null;
        localPos = center.localPos + new Vector2Int(1, -1);
        if (localPos.x < request.gridSize && downTraversable && rightTraversable)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                node = new Node(request.informer.IsTraversable(pos), localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[5] = node;

        // down left: -1, -1
        node = null;
        localPos = center.localPos + new Vector2Int(-1, -1);
        if (localPos.x < request.gridSize && downTraversable && leftTraversable)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                node = new Node(request.informer.IsTraversable(pos), localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[6] = node;

        // up left: -1, 1
        node = null;
        localPos = center.localPos + new Vector2Int(-1, 1);
        if (localPos.x < request.gridSize && upTraversable && leftTraversable)
        {
            if (!grid.TryGetValue(localPos, out node))
            {
                pos = LocalToGlobalPos(request, localPos);
                node = new Node(request.informer.IsTraversable(pos), localPos);
                grid.Add(node.localPos, node);
            }
        }
        neighbours[7] = node;
    }

    private static Vector3 LocalToGlobalPos(PathRequest request, Vector2Int localPos)
    {
        Vector3Int gridOrigin = request.start;
        gridOrigin.x -= request.gridSize / 2;
        gridOrigin.z -= request.gridSize / 2;

        Vector3 globalPos = gridOrigin;
        globalPos.x += localPos.x;
        globalPos.z += localPos.y;
        globalPos /= request.resolution;
        return globalPos;
    }
}

public class PathRequest
{
    public readonly int gridSize;
    public readonly float resolution;
    public readonly Vector3Int start;
    public readonly Vector3Int target;
    public readonly TerrainInformer informer;
    private bool cancelled = false;
    public bool Cancelled { get { return cancelled; } }

    public void Cancel() { cancelled = true; }

    public PathRequest(Vector3 start, Vector3 target, int gridSize, float resolution, TerrainInformer informer)
    {
        resolution = Mathf.Max(resolution, 0.1f);
        start *= resolution;
        target *= resolution;
        this.start = Vector3Int.RoundToInt(start);
        this.target = Vector3Int.RoundToInt(target);
        this.target.y = this.start.y;
        this.gridSize = Mathf.Max(gridSize, 1);
        this.resolution = resolution;
        this.informer = informer;
    }
}

public class PathResult : IEnumerable<Vector3>
{
    private Vector3[] path;
    public readonly PathRequest request;
    public readonly bool success;

    public int Length { get { return path.Length; } }
    public Vector3[] Path { get { return path; } }

    public PathResult(Vector3[] path, bool success, PathRequest request)
    {
        this.path = path;
        this.success = success;
        this.request = request;
    }

    public Vector3 Get(int idx)
    { 
        return path[idx];
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        return ((IEnumerable<Vector3>)path).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return path.GetEnumerator();
    }

    public override string ToString()
    {
        return $"PathResult: success={success}, Length={Length}";
    }
}

public class ColliderInformer : TerrainInformer
{
    private Vector3 size;
    private Collider[] colliders;
    private ulong[] ulongs;
    private Vector3 origin;
    private Vector3Int scaledOrigin;
    private float resolution;
    private int gridSize;
    private int mask;

    public ColliderInformer(Vector3 size, int mask)
    {
        this.mask = mask;
        this.size = size / 2;
        colliders = new Collider[1];
    }

    public bool IsTraversable(Vector3 position)
    {
        Vector3Int localPos = Vector3Int.RoundToInt(position * resolution) - scaledOrigin;
        int x = Mathf.RoundToInt(position.x * resolution) - scaledOrigin.x;
        int y = Mathf.RoundToInt(position.z * resolution) - scaledOrigin.z;
        int rawIdx = y * gridSize + x;
        int idx = rawIdx >> 6;
        if (idx < 0 || idx >= ulongs.Length)
            return false;
        return (ulongs[idx] & (1ul << (rawIdx % 64))) > 0 ? true : false;
    }

    public void Bake(Vector3 center, int gridSize, float resolution)
    {
        this.gridSize = gridSize;
        this.resolution = resolution;
        scaledOrigin = Vector3Int.RoundToInt(center * resolution) - new Vector3Int(gridSize / 2, 0, gridSize / 2);
        origin = (Vector3)scaledOrigin / resolution;

        int size = Mathf.CeilToInt(gridSize * gridSize / 64.0f);
        ulongs = new ulong[size];
        for (int y = 0; y < gridSize; ++y)
        {
            for (int x = 0; x < gridSize; ++x)
            {
                int index = (y * gridSize + x) >> 6;
                int i = (y * gridSize + x) % 64;
                ulongs[index] &= ~(1ul << i);
                ulongs[index] |= Check(origin + new Vector3(x / resolution, 0, y / resolution)) << i;
            }
        }
    }

    private ulong Check(Vector3 position)
    {
        return Physics.OverlapBoxNonAlloc(position, size, colliders, Quaternion.identity, mask) == 0 ? 1ul : 0;
    }
}