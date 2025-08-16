using Newtonsoft.Json.Linq;
using UnityEngine;

public class Entry : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 100;

        JObject data = new JObject();
        SceneCodec.Serialize(data);
        Debug.Log(data);
    }
}
