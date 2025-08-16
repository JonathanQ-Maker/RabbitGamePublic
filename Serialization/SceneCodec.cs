using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public static class SceneCodec
{
    public static void Serialize(JObject sceneData)
    {
        JArray root = new JArray();
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject child in rootObjects)
        {
            if (child.TryGetComponent(out SerializablePrefab serializable))
            { 
                JObject json = new JObject
                {
                    { "PrefabName", serializable.PrefabName }
                };
                serializable.Serialize(json);
                root.Add(json);
            }
        }
        sceneData["objects"] = root;
    }

    public static void Deserialize(JObject sceneData, SerializablePrefabRegistry prefabRegistry)
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject child in rootObjects)
        {
            if (child.TryGetComponent(out SerializablePrefab serializable))
                GameObject.Destroy(child);
        }

        foreach (JObject child in (JArray)sceneData["objects"])
        {
            SerializablePrefab prefab = prefabRegistry.Get(child.Value<string>("PrefabName"));
            Object.Instantiate(prefab).Deserialize(child);
        }
    }
}
