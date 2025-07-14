using UnityEngine;

public static class Extensions
{
    public static bool CheckDestroyed(this object obj)
    {
        if (obj is MonoBehaviour)
            return obj as MonoBehaviour == null;
        return obj == null;
    }
}