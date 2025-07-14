using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private static PathRequestManager instance;

    private void Awake()
    {
        if (!ReferenceEquals(instance, null))
        { 
            Destroy(instance);
            Debug.LogWarning($"Multiple instances of PathRequestManager");
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance);
    }

    private Queue<KeyValuePair<PathResult, Action<PathResult>>> results = new Queue<KeyValuePair<PathResult, Action<PathResult>>>();

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; ++i)
                {
                    KeyValuePair<PathResult, Action<PathResult>> result = results.Dequeue();

                    if (!result.Key.request.Cancelled)
                    {
                        result.Value.Invoke(result.Key);
                    }
                }
            }
        }
    }

    private void PrivateRequestPath(PathRequest request, Action<PathResult> callback)
    {
        Task.Run(() => {
            PathResult result = PathFinding.FindPath(request);
            if (result == null) return;
            lock (results)
            { 
                results.Enqueue(new KeyValuePair<PathResult, Action<PathResult>>(result, callback));
            }
        });
    }

    public static void RequestPath(PathRequest request, Action<PathResult> callback)
    { 
        instance.PrivateRequestPath(request, callback);
    }
}