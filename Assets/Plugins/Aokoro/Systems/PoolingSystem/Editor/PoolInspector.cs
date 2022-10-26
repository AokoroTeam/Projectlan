using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes.Editor;

namespace Aokoro.Pooling.Editor
{
    [CustomEditor(typeof(Pool), true)]
    public class PoolInspector : UnityEditor.Editor
    {
        SerializedProperty capacity;
        SerializedProperty loadOnAwake;
        SerializedProperty resource;

        private void OnEnable()
        {
            capacity = serializedObject.FindProperty("m_capacity");
            loadOnAwake = serializedObject.FindProperty("m_loadOnAwake");
            resource = serializedObject.FindProperty("resource");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(capacity, new GUIContent("Capacity"));
            EditorGUILayout.PropertyField(loadOnAwake, new GUIContent("Load on awake"));
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(resource, new GUIContent("Resource"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}