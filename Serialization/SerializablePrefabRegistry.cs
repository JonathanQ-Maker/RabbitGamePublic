using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "ScriptableObjects/SerializablePrefabRegistry", order = 1)]
public class SerializablePrefabRegistry : ScriptableObject
{
    [SerializeField] private List<SerializablePrefab> values = new List<SerializablePrefab>();

    private Dictionary<string, SerializablePrefab> dictionary = new Dictionary<string, SerializablePrefab>();






    public SerializablePrefab Get(string key)
    {
        TrySync();
        return dictionary[key];
    }

    public bool Contains(string key)
    {
        TrySync();
        return dictionary.ContainsKey(key);
    }

    public bool TryGet(string key, out SerializablePrefab prefab)
    {
        TrySync();
        return dictionary.TryGetValue(key, out prefab);
    }

    public void TrySync()
    {
        if (values.Count == dictionary.Count) return;

        dictionary.Clear();
        foreach (SerializablePrefab prefab in values)
        {
            dictionary.Add(prefab.PrefabName, prefab);
        }
    }
}