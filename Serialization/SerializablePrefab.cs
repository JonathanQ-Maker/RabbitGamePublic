using Newtonsoft.Json.Linq;
using UnityEngine;

public class SerializablePrefab : MonoBehaviour
{
    [SerializeField]
    private string prefabName;

    public string PrefabName { get { return prefabName; } }

    public void Serialize(JObject data)
    {
        JArray transformData = new JArray { 
            transform.localPosition.x,
            transform.localPosition.y,
            transform.localPosition.z,

            transform.localRotation.x,
            transform.localRotation.y,
            transform.localRotation.z,
            transform.localRotation.w,

            transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        };
        data.Add("transform", transformData);
        if (TryGetComponent(out IJsonSerializable serializable))
        { 
            serializable.Serialize(data);
        }
    }

    public void Deserialize(JObject data)
    {
        float[] transformData = data["transform"].ToObject<float[]>();
        Vector3 position = new Vector3(transformData[0], transformData[1], transformData[2]);
        Quaternion rotation = new Quaternion(transformData[3], transformData[4], transformData[5], transformData[6]);
        Vector3 scale = new Vector3(transformData[7], transformData[8], transformData[9]);
        transform.SetLocalPositionAndRotation(position, rotation);
        transform.localScale = scale;
        if (TryGetComponent(out IJsonSerializable serializable))
        {
            serializable.Deserialize(data);
        }
    }
}