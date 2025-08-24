using System.IO;
using UnityEditor;
using UnityEngine;

public class PreviewGenerator : EditorWindow
{
    private GameObject prefab;
    [MenuItem("Window/Preview Generator")]
    static void Init()
    {
        GetWindow<PreviewGenerator>().Show();
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (prefab == null) return;

        // GetAssetPreview generates a "rendered" thumbnail (can take a frame or two)
        Texture2D previewTexture = AssetPreview.GetAssetPreview(prefab);

        // If not ready yet, fall back to mini thumbnail
        if (previewTexture == null)
            previewTexture = AssetPreview.GetMiniThumbnail(prefab);


        GUILayout.Label(previewTexture, GUILayout.Width(100), GUILayout.Height(100));

        if (GUILayout.Button("Save Preview"))
        {
            string path = EditorUtility.SaveFilePanel("Save Preview To", Application.dataPath, $"{prefab.name}_icon", "png");
            if (path.Length != 0)
            {
                SaveTextureAsPNG(previewTexture, path);
            }
        }
    }

    public void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}