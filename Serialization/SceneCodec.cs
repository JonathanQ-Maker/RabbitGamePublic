using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public static class SceneCodec
{
    public static JArray Serialize()
    {
        JArray root = new JArray();
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject child in rootObjects)
        {
            if (child.TryGetComponent(out IJsonSerializable serializable))
            { 
                JObject json = new JObject
                {
                    { "type", serializable.GetType().FullName }
                };
                serializable.Serialize(json);
                root.Add(json);
            }
        }
        return root;
    }

    public static void Deserialize(JArray root)
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject child in rootObjects)
        {
            if (child.TryGetComponent(out IJsonSerializable serializable))
                GameObject.Destroy(child);
        }

        foreach (JObject child in root)
        {
            Type type = Type.GetType(child.Value<string>("type"));
            if (typeof(IJsonSerializable).IsAssignableFrom(type))
            {
                GameObject newGameObject = new GameObject();
                IJsonSerializable serializable = newGameObject.AddComponent(type) as IJsonSerializable;
                serializable.Deserialize(child);
            }
            else
            {
                Debug.LogWarning($"Tried to deserialize an incompatible type \"{child.Value<string>("type")}\"");
            }
        }
    }
}

public interface IJsonSerializable
{
    void Serialize(JObject json);
    void Deserialize(JObject json);
}
