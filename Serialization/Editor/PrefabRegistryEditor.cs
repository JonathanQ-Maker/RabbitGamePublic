using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SerializablePrefabRegistry))]
public class PrefabRegistryEditor : Editor
{
    private string removeName = string.Empty;
    private SerializablePrefab prefab;

    private SerializablePrefabRegistry registry;
    private SerializedProperty valuesProp;
    private void OnEnable()
    {
        registry = (SerializablePrefabRegistry)target;
        valuesProp = serializedObject.FindProperty("values");

        ClearInvalid();
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawAddPrefab();
        EditorGUILayout.Space();
        DrawPrefabsDisplay();
        serializedObject.ApplyModifiedProperties();
    }

    private bool HasPrefab(SerializablePrefab prefab)
    {
        for (int i = 0; i < valuesProp.arraySize; ++i)
        {
            SerializedProperty prefabProp = valuesProp.GetArrayElementAtIndex(i);
            if (ReferenceEquals(prefabProp.objectReferenceValue, prefab))
            {
                return true;
            }
        }
        return false;
    }

    private void AddPrefab(SerializablePrefab prefab)
    {
        valuesProp.arraySize++;
        SerializedProperty newValue = valuesProp.GetArrayElementAtIndex(valuesProp.arraySize - 1);
        newValue.objectReferenceValue = prefab;
    }

    private void ClearInvalid()
    {
        for (int i = 0; i < valuesProp.arraySize; ++i)
        {
            if (valuesProp.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                valuesProp.DeleteArrayElementAtIndex(i);
                --i;
            }
        }
    }


    private void DrawAddPrefab()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Add Prefab");
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(SerializablePrefab), false) as SerializablePrefab;

        EditorGUI.BeginDisabledGroup(prefab == null || HasPrefab(prefab));
        if (GUILayout.Button("Add Prefab"))
        {
            AddPrefab(prefab);
            prefab = null;
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }

    private void DrawPrefabsDisplay()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField($"Prefabs ({valuesProp.arraySize})");
        // Draw Prefabs
        for (int i = valuesProp.arraySize-1; i >= 0; --i)
        {
            SerializedProperty valueElement = valuesProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            Texture preview = AssetPreview.GetAssetPreview(((MonoBehaviour)valueElement.objectReferenceValue).gameObject);
            GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(((SerializablePrefab)valueElement.objectReferenceValue).PrefabName);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(valueElement.objectReferenceValue, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Remove"))
            {
                valuesProp.DeleteArrayElementAtIndex(i);
                return;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
