using UnityEngine;

[RequireComponent(typeof(PathRequestManager))]
[RequireComponent(typeof(PathFindingDebug))]
public class PathFindingExample : MonoBehaviour
{
    public Transform start, target;
    public float resolution;
    public int gridSize;
    public Vector3 scanSize;
    public int mask = 1;

    public bool findPathSync = false;
    public bool findPathAsync = false;

    private PathResult result;
    private ColliderInformer informer;
    private PathRequestManager manager;
    private PathFindingDebug debug;

    private void Start()
    {
        manager = GetComponent<PathRequestManager>();
        debug = GetComponent<PathFindingDebug>();
        informer = new ColliderInformer(scanSize, mask);
    }
    void Update()
    {
        if (findPathSync)
        {
            findPathSync = false;
            FindPathSync();
        }

        if (findPathAsync)
        { 
            findPathAsync = false;
            FindPathAsync();
        }
    }

    public void FindPathSync()
    {
        informer.Bake(start.transform.position, gridSize, resolution);

        PathRequest request = new PathRequest(start.position, target.position, gridSize, resolution, informer);
        HandleResult(PathFinding.FindPath(request));
    }

    public void FindPathAsync()
    {
        informer.Bake(start.transform.position, gridSize, resolution);
        PathRequest request = new PathRequest(start.position, target.position, gridSize, resolution, informer);
        PathRequestManager.RequestPath(request, HandleResult);
    }

    private void HandleResult(PathResult result)
    {
        this.result = result;
        debug.Result = result;
        Debug.Log($"Path result: length = {result.Length}, success = {result.success}");
    }

}
