using Aokoro.ModelExports;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor.Presets;

namespace Realit.Library.Editor
{
    [CreateAssetMenu(fileName = "RealitLibrarySettings", menuName = "Aokoro/Realit/Library/Settings")]
    public class RealitLibrarySettings : ScriptableObject
    {

        //Const
        private const string PrefabFolder = "Prefabs";
        private const string MaterialFolder = "Materials";
        private const string ModelFolder = "Meshes";

        //Static
        public static string PrefabFolderPath => CreateOrGetLibraryFolder(PrefabFolder);
        public static string MaterialFolderPath => CreateOrGetLibraryFolder(MaterialFolder);
        public static string ModelFolderPath => CreateOrGetLibraryFolder(ModelFolder);
        public static string LibraryFolderPath => Path.GetDirectoryName(AssetDatabase.GetAssetPath(Instance));
        
        private static RealitLibrarySettings instance;
        public static RealitLibrarySettings Instance
        {
            get
            {
                if (instance == null)
                {
                    //Find existing
                    var results = AssetDatabase.FindAssets($"t:{nameof(RealitLibrarySettings)}");
                    if (results.Length == 0)
                    {
                        instance = CreateInstance<RealitLibrarySettings>();

                        AssetDatabase.CreateAsset(instance, Directory.Exists("Assets/Data/Library") ? "Assets/Data/Library/RealitLibrarySettings.asset" : "Assets/RealitLibrarySettings.asset");
                        AssetDatabase.SaveAssets();

                        EditorUtility.FocusProjectWindow();
                    }
                    else
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        instance = AssetDatabase.LoadAssetAtPath<RealitLibrarySettings>(path);
                    }
                }

                return instance;
            }
        }

        //Local
        [BoxGroup("Addressables"), SerializeField]
        private AddressableAssetGroupTemplate realitAdressableGroupTemplate;

        [BoxGroup("Models"), SerializeField]
        private Preset modelImportPreset;

        #region Folder management
        private static string CreateOrGetLibraryFolder(string folderName)
        {
            string fullPath = Path.Combine(LibraryFolderPath, folderName);
            if (!AssetDatabase.IsValidFolder(fullPath))
                return CreateLibraryFolder(folderName);
            else
                return fullPath;
        }


        private static string CreateLibraryFolder(string folderName) => AssetDatabase.CreateFolder(LibraryFolderPath, folderName);

        public static bool IsInLibraryFolder(string path) => Path.GetRelativePath(LibraryFolderPath, path) != path;
        public static bool IsInPrefabFolder(string path) => Path.GetRelativePath(PrefabFolderPath, path) != path;
        public static bool IsInMaterialFolder(string path) => Path.GetRelativePath(MaterialFolderPath, path) != path;
        public static bool IsInModelFolder(string path) => Path.GetRelativePath(ModelFolderPath, path) != path;
        #endregion

        #region Ressource Management
       
        public static void GenerateRessources(params string[] prefabsPath)
        {
            List<ModelExport> exports = new();
            List<string> retainedPaths = new List<string>();

            for (int i = 0; i < prefabsPath.Length; i++)
            {
                string prefabPath = prefabsPath[i];
                if (IsInPrefabFolder(prefabPath))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    string relativePath = Path.GetRelativePath(PrefabFolderPath, Path.GetDirectoryName(prefabPath));
                    string exportPath = Path.Combine(ModelFolderPath, relativePath, $"{prefab.name}.fbx");

                    ModelExport item = new ModelExport(prefab, exportPath);
                    item.OnModelExported += path =>
                    {
                        var importer = ModelImporter.GetAtPath(path);
                        instance.modelImportPreset.ApplyTo(importer);
                        importer.SaveAndReimport();
                    };

                    exports.Add(item);

                    continue;
                }
                
                RealitLibrary.LogError($"{prefabPath} is not in library. Please move the object in the library's prefab folder and try again.");
            }

            ModelExport.Export(exports.ToArray());
        }


        [MenuItem("Realit/Library/Generate")]
        public static void Generate() => Instance.GenerateLibrary();

        [Button]
        public void GenerateLibrary()
        {
            const string groupName = "Realit_Library";
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!settings)
                return;

            var entries = new List<AddressableAssetEntry>();

            //Clearing addressables
            AddressableAssetGroup group = SetupAddressableGroup(groupName, settings, entries);

            //Getting all prefabs
            var guids = AssetDatabase.FindAssets("t:prefab", new string[] { PrefabFolderPath });
            int length = guids.Length;

            List<string> prefabPaths = new List<string>();

            for (int i = 0; i < length; i++)
            {
                string guid = guids[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //Adding scripts
                FillPrefabWithScripts(guid, assetPath);

                //Setting as addressable
                entries.Add(settings.CreateOrMoveEntry(guid, group, false, false));

                prefabPaths.Add(assetPath);
            }

            GenerateRessources(prefabPaths.ToArray());

            settings.SetDirty(
                AddressableAssetSettings.ModificationEvent.EntryMoved |
                AddressableAssetSettings.ModificationEvent.EntryCreated |
                AddressableAssetSettings.ModificationEvent.EntryRemoved,
                entries, true);
            
            AssetDatabase.Refresh();
        }
        private AddressableAssetGroup SetupAddressableGroup(string groupName, AddressableAssetSettings settings, List<AddressableAssetEntry> entries)
        {
            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, realitAdressableGroupTemplate.SchemaObjects);
            else
                realitAdressableGroupTemplate.ApplyToAddressableAssetGroup(group);

            group.GatherAllAssets(entries, true, false, false); 

            foreach (var item in entries)
                settings.RemoveAssetEntry(item.guid, false);

            entries.Clear();
            return group;
        }

        private static void FillPrefabWithScripts(string guid, string assetPath)
        {
            GameObject prefab = PrefabUtility.LoadPrefabContents(assetPath);

            if (!prefab.TryGetComponent(out RealitAsset realitLibraryPrefab))
                realitLibraryPrefab = prefab.AddComponent<RealitAsset>();

            realitLibraryPrefab.projectPath = assetPath;
            realitLibraryPrefab.addressableKey = guid;

            PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
            PrefabUtility.UnloadPrefabContents(prefab);
        }
        #endregion
    }
}
