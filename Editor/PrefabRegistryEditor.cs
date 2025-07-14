using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PrefabRegistry))]
public class PrefabRegistryEditor : Editor
{
    private string removeKey = string.Empty;
    private GameObject prefab;

    private PrefabRegistry registry;
    private SerializedProperty keysProp;
    private SerializedProperty valuesProp;
    private HashSet<string> keys = new HashSet<string>();
    private void OnEnable()
    {
        registry = (PrefabRegistry)target;
        keysProp = serializedObject.FindProperty("keys");
        valuesProp = serializedObject.FindProperty("values");

        ClearInvalid();
        for (int i = 0; i < keysProp.arraySize; ++i)
        {
            SerializedProperty element = keysProp.GetArrayElementAtIndex(i);
            keys.Add(element.stringValue);
        }
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawAddPrefab();
        EditorGUILayout.Space();
        DrawRemovePrefab();
        EditorGUILayout.Space();
        DrawPrefabsDisplay();
        serializedObject.ApplyModifiedProperties();
    }

    private void AddPrefab(string key, GameObject prefab)
    {
        keysProp.arraySize++;
        valuesProp.arraySize++;
        SerializedProperty newKey = keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1);
        SerializedProperty newValue = valuesProp.GetArrayElementAtIndex(valuesProp.arraySize - 1);
        newKey.stringValue = key;
        newValue.objectReferenceValue = prefab;
        keys.Add(key);
    }

    private void RemovePrefab(string key)
    {
        for (int i = 0; i < keysProp.arraySize; ++i)
        {
            SerializedProperty keyElement = keysProp.GetArrayElementAtIndex(i);
            if (keyElement.stringValue.Equals(key))
            {
                keys.Remove(keyElement.stringValue);
                keysProp.DeleteArrayElementAtIndex(i);
                valuesProp.DeleteArrayElementAtIndex(i);
                break;
            }
        }
    }

    private void ClearInvalid()
    {
        for (int i = 0; i < valuesProp.arraySize; ++i)
        {
            if (valuesProp.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                valuesProp.DeleteArrayElementAtIndex(i);
                keysProp.DeleteArrayElementAtIndex(i);
                --i;
            }
        }
    }


    private void DrawAddPrefab()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Add Prefab");
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;

        EditorGUI.BeginDisabledGroup(prefab == null || keys.Contains(prefab.name));
        if (GUILayout.Button("Add Prefab"))
        {
            AddPrefab(prefab.name, prefab);
            prefab = null;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }

    private void DrawRemovePrefab()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Remove Prefab");
        removeKey = EditorGUILayout.TextField("Key", removeKey).Trim();
        EditorGUI.BeginDisabledGroup(!keys.Contains(removeKey));
        if (GUILayout.Button("Remove Prefab"))
        {
            RemovePrefab(removeKey);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }

    private void DrawPrefabsDisplay()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField($"Prefabs ({valuesProp.arraySize})");
        EditorGUI.BeginDisabledGroup(true);
        // Draw Prefabs
        for (int i = 0; i < valuesProp.arraySize; ++i)
        {
            SerializedProperty valueElement = valuesProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            Texture preview = AssetPreview.GetAssetPreview(valueElement.objectReferenceValue);
            GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
            EditorGUILayout.ObjectField(valueElement.objectReferenceValue, typeof(GameObject), false);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }
}
