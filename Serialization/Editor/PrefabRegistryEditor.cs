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
        DrawRemovePrefab();
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

    private bool HasPrefab(string prefabName)
    {
        for (int i = 0; i < valuesProp.arraySize; ++i)
        {
            SerializablePrefab prefab = valuesProp.GetArrayElementAtIndex(i).objectReferenceValue as SerializablePrefab;
            if (prefab.PrefabName.Equals(prefabName))
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

    private void RemovePrefab(string prefabName)
    {
        for (int i = 0; i < valuesProp.arraySize; ++i)
        {
            SerializablePrefab prefab = valuesProp.GetArrayElementAtIndex(i).objectReferenceValue as SerializablePrefab;
            if (prefab.PrefabName.Equals(prefabName))
            {
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

    private void DrawRemovePrefab()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Remove Prefab");
        removeName = EditorGUILayout.TextField("Name", removeName).Trim();
        EditorGUI.BeginDisabledGroup(!HasPrefab(removeName));
        if (GUILayout.Button("Remove Prefab"))
        {
            RemovePrefab(removeName);
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
            Texture preview = AssetPreview.GetAssetPreview(((MonoBehaviour)valueElement.objectReferenceValue).gameObject);
            GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));

                EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(((SerializablePrefab)valueElement.objectReferenceValue).PrefabName);
                    EditorGUILayout.ObjectField(valueElement.objectReferenceValue, typeof(GameObject), false);
                EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();
    }
}
