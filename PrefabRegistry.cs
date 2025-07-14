using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs", menuName = "ScriptableObjects/PrefabRegistry", order = 1)]
public class PrefabRegistry : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<GameObject> values = new List<GameObject>();

    private Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();






    public GameObject Get(string key)
    { 
        return dictionary[key];
    }

    public bool Contains(string key)
    { 
        return dictionary.ContainsKey(key);
    }

    public bool TryGet(string key, out GameObject prefab)
    { 
        return dictionary.TryGetValue(key, out prefab);
    }






    public void OnBeforeSerialize()
    {
        keys.Clear(); 
        values.Clear();
        foreach (KeyValuePair<string, GameObject> pair in dictionary)
        { 
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        for (int i = 0; i < keys.Count; ++i)
        {
            dictionary.Add(keys[i], values[i]);
        }
    }
}