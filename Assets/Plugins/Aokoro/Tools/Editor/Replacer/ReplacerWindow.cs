using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Search;

namespace Aokoro.Editor.Replacer
{
    public class ReplacerWindow : EditorWindow
    {
        private GameObject prefab;
        private int tab;
        [SerializeField]
        private GameObject[] targets;

        [MenuItem("Tools/Aokoro/Replacer")]
        static void CreateReplaceWithPrefab()
        {
            EditorWindow.GetWindow<ReplacerWindow>("Replacer");
        }

        private void Awake()
        {
            Selection.selectionChanged += Repaint;
        }

        private void OnDestroy()
        {
            Selection.selectionChanged -= Repaint;
        }

        private void OnGUI()
        {

            GUILayout.BeginVertical("GroupBox");

            int newTab = GUILayout.Toolbar(tab, new string[] { "Selection", "Instance of" /*, "Search" */});
            EditorGUILayout.Space();
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

            if (newTab != tab)
                targets = new GameObject[0];

            tab = newTab;
            switch (tab)
            {
                case 0:
                    SelectionReplacement();
                    break;
                case 1:
                    InstanceOfReplacement();
                    break;
                default:
                    return;
            }
            GUILayout.EndVertical();

        }


        private void SelectionReplacement()
        {
            int selectionCount = Selection.gameObjects.Length;
            if (selectionCount == 0)
            {
                EditorGUILayout.HelpBox("Please, select at least one object.", MessageType.Error);
            }
            else
            {
                targets = Selection.gameObjects;
                DrawTargets();

                if (DrawReplaceButton())
                    Replace();
            }
        }

        //TODO
        private GameObject prefabInstanceSource;
        private void InstanceOfReplacement()
        {
            var _source = (GameObject)EditorGUILayout.ObjectField("Source", prefabInstanceSource, typeof(GameObject), false);
            if (_source == null)
            {
                if (prefabInstanceSource != null)
                    targets = new GameObject[0];

                prefabInstanceSource = null;
                EditorGUILayout.HelpBox("Source cannot be empty.", MessageType.Warning);
                return;
            }

            var prefabType = PrefabUtility.GetPrefabAssetType(_source);
            bool valid = prefabType.HasFlag(PrefabAssetType.Model) || prefabType.HasFlag(PrefabAssetType.Regular) || prefabType.HasFlag(PrefabAssetType.Variant);

            if (!valid)
                EditorGUILayout.HelpBox("Source cannot be empty.", MessageType.Warning);
            else
            {
                //Search for all references
                EditorGUILayout.Separator();
                if (_source != prefabInstanceSource || GUILayout.Button("Refresh", EditorStyles.miniButtonLeft))
                    targets = PrefabUtility.FindAllInstancesOfPrefab(_source);

                prefabInstanceSource = _source;
                DrawTargets();

                if (DrawReplaceButton())
                    Replace();
            }
        }


        //TODO
        private void SearchReplacement()
        {
            EditorGUILayout.HelpBox("Not implemented yet", MessageType.Warning);
        }

        private void Replace()
        {
            if (targets == null)
                return;

            GameObject[] instances = new GameObject[targets.Length];
            Undo.SetCurrentGroupName($"Replacement of {targets.Length} objects.");
            int group = Undo.GetCurrentGroup();

            for (var i = targets.Length - 1; i >= 0; --i)
            {
                var selected = targets[i];
                var prefabType = PrefabUtility.GetPrefabAssetType(prefab);

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, selected.scene);

                instance.transform.parent = selected.transform.parent;
                instance.transform.localPosition = selected.transform.localPosition;
                instance.transform.localRotation = selected.transform.localRotation;
                instance.transform.localScale = selected.transform.localScale;
                instance.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());

                Undo.RegisterCreatedObjectUndo(instance, "Replace With Prefabs");
                Undo.DestroyObjectImmediate(selected);
            }

            Undo.CollapseUndoOperations(group);
            targets = new GameObject[0];
        }


        #region GUI
        private bool DrawReplaceButton()
        {
            EditorGUILayout.Separator();
            if (prefab != null)
                return GUILayout.Button("Replace");
            else
            {
                EditorGUILayout.HelpBox("Please, select the prefab that will replace all targets", MessageType.Error);
                return false;
            }
        }
        bool targetFoldout;

        private void DrawTargets()
        {
            int indent = EditorGUI.indentLevel;
            int length = targets.Length;
            if (length == 0)
                return;
            targetFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(targetFoldout, $"{length} instances found");
            if (targetFoldout)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < length; i++)
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(targets[i], typeof(GameObject), true);
                    GUI.enabled = true;
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel = indent;
        }
        #endregion

    }
}
