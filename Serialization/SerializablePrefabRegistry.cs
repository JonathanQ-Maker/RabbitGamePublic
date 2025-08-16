using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "ScriptableObjects/SerializablePrefabRegistry", order = 1)]
public class SerializablePrefabRegistry : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<SerializablePrefab> values = new List<SerializablePrefab>();

    private Dictionary<string, SerializablePrefab> dictionary = new Dictionary<string, SerializablePrefab>();






    public SerializablePrefab Get(string key)
    { 
        return dictionary[key];
    }

    public bool Contains(string key)
    { 
        return dictionary.ContainsKey(key);
    }

    public bool TryGet(string key, out SerializablePrefab prefab)
    { 
        return dictionary.TryGetValue(key, out prefab);
    }


    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        foreach (SerializablePrefab prefab in values)
        {
            dictionary.Add(prefab.PrefabName, prefab);
        }
    }
}