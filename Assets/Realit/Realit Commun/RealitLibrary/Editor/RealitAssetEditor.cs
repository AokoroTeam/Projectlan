using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Realit.Library.Editor
{
    [CustomEditor(typeof(RealitAsset), true)]
    public class RealitAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty _projectPathProperty;
        public SerializedProperty ProjectPathProperty => _projectPathProperty ??= serializedObject.FindProperty("projectPath");
        
        private SerializedProperty _addressablePathProperty;
        public SerializedProperty AddressablePathProperty => _addressablePathProperty ??= serializedObject.FindProperty("addressableKey");


        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                RealitAsset realitLibraryPrefab = target as RealitAsset;
                GameObject gameObject = realitLibraryPrefab.gameObject;
                realitLibraryPrefab.projectPath = GetPrefabAssetpath(gameObject);
                serializedObject.Update();
            }
        }

        public override void OnInspectorGUI()
        {
            RealitAsset realitLibraryPrefab = target as RealitAsset;

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Library asset", RealitLibrarySettings.Instance, typeof(RealitLibrarySettings), false);
            
            if (!Application.isPlaying)
                EditorGUILayout.PropertyField(ProjectPathProperty);
            
            EditorGUILayout.PropertyField(AddressablePathProperty);
            GUI.enabled = true;
            
            if (!Application.isPlaying)
            {
                string prefabPath = ProjectPathProperty.stringValue;
                if (GUILayout.Button("Generate Assets"))
                    RealitLibrarySettings.GenerateRessources(prefabPath);
            }

            //Apply
            serializedObject.ApplyModifiedProperties();
        }


        #region Utility
        public static string GetPrefabAssetpath(GameObject gameObject)
        {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
                return prefabStage.assetPath;

            if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);

            if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);

            RealitLibrary.LogError($"{gameObject.name} is not part of a prefab");
            return null;

        }

        #endregion
    }
}
