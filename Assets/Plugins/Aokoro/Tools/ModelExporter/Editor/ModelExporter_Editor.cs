using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Aokoro.ModelExports.Runtime;

namespace Aokoro.Tools.ModelExports.Runtime.Editor
{
    [CustomEditor(typeof(ModelExportComponent), editorForChildClasses: true), CanEditMultipleObjects]

    public class ModelExporter_Editor : UnityEditor.Editor
    {
        private const int FirstColumnMaxWidth = 75;

        private SerializedProperty exportPath;
        private SerializedProperty fileName;

        private void OnEnable()
        {
            exportPath = serializedObject.FindProperty("exportPath");
            fileName = serializedObject.FindProperty("fileName");

            DetectPath(target as ModelExportComponent);
        }

        public override void OnInspectorGUI()
        {
            ModelExportComponent meshExporter = (target as ModelExportComponent);
            serializedObject.Update();


            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Folder", GUILayout.MaxWidth(FirstColumnMaxWidth));
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(exportPath, GUIContent.none, GUILayout.ExpandWidth(true));
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("File name", GUILayout.MaxWidth(FirstColumnMaxWidth));
                        EditorGUILayout.PropertyField(fileName, GUIContent.none, GUILayout.ExpandWidth(true));
                        GUI.enabled = false;
                        EditorGUILayout.TextField(".fbx", GUILayout.MaxWidth(50));
                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("Change path", GUILayout.ExpandHeight(true)))
                {
                    string name = string.IsNullOrWhiteSpace(fileName.stringValue) ? meshExporter.gameObject.name : fileName.stringValue;
                    string fullPath = EditorUtility.SaveFilePanelInProject("Select file path", name, "fbx", "", exportPath.stringValue);

                    exportPath.stringValue = Path.GetRelativePath("Assets", Path.GetDirectoryName(fullPath));
                    fileName.stringValue = Path.GetFileNameWithoutExtension(fullPath);

                    serializedObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("Detect path", GUILayout.ExpandHeight(true)))
                {
                    DetectPath(meshExporter);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Export", EditorStyles.miniButtonMid))
            {
                meshExporter.TryExport();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DetectPath(ModelExportComponent meshExporter)
        {
            var meshFilters = meshExporter.GetComponentsInChildren<MeshFilter>();

            for (int i = 0; i < meshFilters.Length; i++)
            {
                Mesh sharedMesh = meshFilters[i].sharedMesh;
                if (sharedMesh != null)
                {
                    string fullPath = AssetDatabase.GetAssetPath(sharedMesh);
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        exportPath.stringValue = Path.GetRelativePath("Assets", Path.GetDirectoryName(fullPath));
                        fileName.stringValue = Path.GetFileNameWithoutExtension(fullPath);
                        break;
                    }
                }
            }
        }
    }
}