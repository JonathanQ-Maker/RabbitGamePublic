
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MenuItems
{
    [MenuItem("DevTools/Save Scene")]
    public static void SaveScene()
    {
        string savePath = Path.Combine(Application.dataPath, "Resources", "Scenes", "main.json");
        JArray scene = SceneCodec.Serialize();
        using (StreamWriter writer = new StreamWriter(savePath))
        using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
        {
            // Optional: Makes the JSON output pretty
            jsonWriter.Formatting = Formatting.Indented;
            scene.WriteTo(jsonWriter);
            Debug.Log($"Saved scene to \"{savePath}\"");
        }
    }

    [MenuItem("DevTools/Load Scene")]
    public static void LoadScene() 
    {
        string savePath = Path.Combine(Application.dataPath, "Resources", "Scenes", "main.json");

        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"Cannot load scene, file path \"{savePath}\" does not exist");
            return;
        }

        float startTime = Time.realtimeSinceStartup;
        using (StreamReader reader = new StreamReader(savePath))
        using (JsonTextReader jsonReader = new JsonTextReader(reader))
        {
            JArray array = JArray.Load(jsonReader);
            SceneCodec.Deserialize(array);
            Debug.Log($"Loaded scene from \"{savePath}\", took {Time.realtimeSinceStartup - startTime:F2}s");
        }
    }

    [MenuItem("DevTools/Save Scene", true)]
    [MenuItem("DevTools/Load Scene", true)]
    public static bool IsRunning()
    {
        return Application.isPlaying;
    }
}
