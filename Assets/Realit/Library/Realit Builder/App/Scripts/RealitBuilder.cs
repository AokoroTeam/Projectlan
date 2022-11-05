using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using NaughtyAttributes;
using Michsky.UI.ModernUIPack;

using Realit.Scene;

using Aokoro;
using SFB;

namespace Realit.Builder.App
{
    public enum Phase
    {
        Import = 0,
        Edition = 1
    }

    [AddComponentMenu("Realit/Headless Builder/Realit Builder (App)")]
    public class RealitBuilder : RealitBuilderBase
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);
#endif
        public static new RealitBuilder Instance => GetInstance<RealitBuilder>();

        [SerializeField]
        private WindowManager windowManager;
        //Phases
        public static event Action<Phase> OnPhaseChanges;
        public Phase CurrentPhase { get; private set; }

        //Building
        [SerializeField]
        private RealitScene scene;

        public static bool IsProjectSetup => Instance.Scene.IsValid(DataSection.Project);
        public static bool IsPlayerSetup => Instance.Scene.IsValid(DataSection.Player);
        public static bool IsModelSetup => Instance.Scene.IsValid(DataSection.Model);
        public static bool AreAssetSetup => Instance.Scene.IsValid(DataSection.Assets);

        public static bool CanBuild => IsPlayerSetup && IsModelSetup && IsProjectSetup;

        public RealitScene Scene { get => scene??= new(); }
        

        private void Start()
        {
            ChangePhase(Phase.Import);
        }

#region Edition
        public void ChangePhase(Phase newPhase)
        {
            if (CurrentPhase != newPhase)
            {
                Debug.Log($"[Realit Builder] Going to {newPhase}.");

                CurrentPhase = newPhase;
                switch (newPhase)
                {
                    case Phase.Import:
                        windowManager.OpenWindow("Import");
                        ModelImporter.Instance.LoadMesh();

                        break;
                    case Phase.Edition:
                        windowManager.OpenWindow("Edition");
                        break;
                }

                OnPhaseChanges?.Invoke(newPhase);
            }
        }
#endregion

#if UNITY_EDITOR
        [Button]
        public void BuildUncompressed() => Build(false, Newtonsoft.Json.Formatting.Indented);
#endif

        [Button]
        public void Build(bool compressed = true, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            var buildData = Scene.BuildData(compressed, formatting);
            string projectName = ProjectDataBuilder.ProjectName;

#if UNITY_WEBGL && !UNITY_EDITOR
                //function(gameObjectNamePtr, methodNamePtr, filenamePtr, byteArray, byteArraySize)
                DownloadFile(gameObject.name, nameof(OnUpload), $"{projectName}.rsz", buildData, buildData.Length);
#else
            string path = StandaloneFileBrowser.SaveFilePanel("Export", "", projectName, "rsz");
            if (string.IsNullOrEmpty(path))
                return;

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {

                // Write the data to the file, byte by byte.
                for (int i = 0; i < buildData.Length; i++)
                    fileStream.WriteByte(buildData[i]);
            }
#endif
        }

        private void OnEnable()
        {
            ModelImporter.OnModelLoaded += OnModelLoaded;
        }
        private void OnDisable()
        {
            ModelImporter.OnModelLoaded -= OnModelLoaded;
        }

        private void OnModelLoaded(GameObject model)
        {
            Instance.ChangePhase(model == null ? Phase.Import : Phase.Edition);
        }

        public void OnUpload()
        {
            Debug.Log("Uploaded");
        }

        
    }
}
