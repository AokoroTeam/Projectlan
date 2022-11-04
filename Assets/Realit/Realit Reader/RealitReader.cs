using Aokoro;
using NaughtyAttributes;
using Realit.Reader.Managers;
using Realit.Scene;
using Realit.UI;
using SFB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System.IO;
using Realit.Settings;

namespace Realit.Reader
{
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Realit/Reader/RealitReader")]
    public class RealitReader : Singleton<RealitReader>
    {
        public event Action OnStartsLoading;
        public event Action OnEndsLoading;

        [BoxGroup("Ressources")]
        [SerializeField] AssetReferenceGameObject canvasManagerAsset;
        [BoxGroup("Ressources")]
        [SerializeField] AssetReferenceGameObject sceneManagerAsset;

        private RealitScene realitScene;

        [HideInInspector]
        public GameObject PlayerRessource;

        [BoxGroup("Runtime"), ReadOnly]
        public bool IsReadyForLoading = false;
        [BoxGroup("Runtime")]
        public LoadingPanel loadingPanel;
        [BoxGroup("Runtime")]
        public bool isDemo;
        [BoxGroup("Runtime"), ShowIf(nameof(isDemo))]
        public Transform spawn;


        protected override void Awake()
        {
            base.Awake();
            realitScene = new RealitScene();

        }
        private void Start()
        {
            StartCoroutine(InitializeAddressables());
        }

        private IEnumerator InitializeAddressables()
        {
            loadingPanel.StartLoadingScreen(this, 100);
            var op = Addressables.InitializeAsync();

            while (!op.IsDone)
            {
                loadingPanel.UpdateBar(this, op.PercentComplete * 100, "Initialisation...");
                yield return null;
            }

            loadingPanel.StopLoadingScreen(this);
            IsReadyForLoading = true;

            Debug.Log("[Realit Reader] adressables intialized");

            if(isDemo)
            {
                StartCoroutine(GenerateScene(null));
            }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
#endif
        // Called from browser
        public void OnFileUpload(string url)
        {
            Debug.Log($"Json url is {url}");
            string realUrl = (string)JArray.Parse(url)[0]["url"];

            LoadZsr(realUrl);
        }
        public void AskForScene()
        {

#if UNITY_WEBGL && !UNITY_EDITOR
            UploadFile(gameObject.name, "OnFileUpload", ".rsz", false);
#else

            Debug.Log("[Realit Reader]Asking path");
            var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "rsz", false);
            if (paths.Length > 0)
            {
                Debug.Log($"[Realit Reader] path is {paths[0]}");
                LoadZsr(paths[0]);
            }
#endif
        }
        #region Scene loading
        public void LoadZsr(string path)
        {
#if UNITY_EDITOR
            byte[] data = File.ReadAllBytes(path);
            StartCoroutine(GenerateScene(data));
#else
            StartCoroutine(ILoadZsrWithAddress(path));
#endif
        }

        private IEnumerator ILoadZsrWithAddress(string url)
        {
            OnStartsLoading?.Invoke();
            Debug.Log($"[Realit Reader] Loading rsz at {url}");

            string absoluteUri = new Uri(url).AbsoluteUri;
            var www = new UnityWebRequest(absoluteUri);
            www.downloadHandler = new DownloadHandlerBuffer();

            var operation = www.SendWebRequest();
            while(!operation.isDone)
            {
                Debug.Log($"[Realit Reader] Loading rsz... {operation.progress}%");
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                StartCoroutine(GenerateScene(results));
            }
            
        }
#endregion

#region Setup
        private IEnumerator GenerateScene(byte[] data)
        {
            OnStartsLoading?.Invoke();
            var canvasOperation = canvasManagerAsset.InstantiateAsync();
            loadingPanel.StartLoadingScreen(this, 100);

            Debug.Log($"[Realit Reader] Loading Canvas...");
            while (!canvasOperation.IsDone)
            {
                Debug.Log($"[Realit Reader] Progress : {canvasOperation.PercentComplete * 100} %");
                loadingPanel.UpdateBar(this, canvasOperation.PercentComplete * 100, "Chargement de la scene...");
                yield return null;
            }
            Debug.Log($"[Realit Reader] Canvas loaded");
            
            Debug.Log($"[Realit Reader] Loading Scene Manager...");
            var sceneManagerOperation = sceneManagerAsset.InstantiateAsync();

            while (!sceneManagerOperation.IsDone)
            {
                loadingPanel.UpdateBar(this, sceneManagerOperation.PercentComplete * 100, "Chargement de la scene...");
                Debug.Log($"[Realit Reader] Progress : {sceneManagerOperation.PercentComplete * 100} %");
                yield return null;
            }
            Debug.Log($"[Realit Reader] Scene Manager loaded");
            if (!isDemo)
            {
                Debug.Log($"[Realit Reader] Creating Scene...");
                loadingPanel.UpdateBar(this, .5f * 100, "Téléchargement du contenu");
                realitScene.ReadData(data);

                while (!realitScene.IsDone)
                    yield return null;
            }
            else
            {
                PlayerDataReader playerDataReader = GetComponent<PlayerDataReader>();
                playerDataReader.InstantiatePlayer(spawn.position, spawn.rotation);
                while (!playerDataReader.IsPlayerInstantiated)
                    yield return null;
            }
            
            Debug.Log($"[Realit Reader] Scene created");

            loadingPanel.UpdateBar(this, .75f * 100, "Initialisation...");
            SceneManager.Instance.InitializeScene(new Features.FeatureDataAsset[0]);

            loadingPanel.StopLoadingScreen(this);

            yield return null;
            OnEndsLoading?.Invoke();
        }
        
        public static void OnModelLoaded(GameObject modelGO)
        {

        }

        public static void OnPlayerInstantiated(GameObject playerGO)
        {

        }
#endregion

        
        protected override void OnExistingInstanceFound(RealitReader existingInstance)
        {
            Destroy(gameObject);
        }

    }
}
