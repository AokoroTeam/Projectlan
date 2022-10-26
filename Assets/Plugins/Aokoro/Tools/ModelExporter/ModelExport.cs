
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using Object = UnityEngine.Object;

namespace Aokoro.ModelExports
{
    public class ModelExport
    {
#if UNITY_EDITOR
        public event Action<string> OnModelExported;

        public string ExportPath { get; private set; }
        public GameObject Target { get; private set; }
        public string PrefabPath { get; private set; }
        public bool IsPrefab { get; private set; }
        public ModelExportOperation Operation { get; internal set; }

        internal OperationStatus OperationStatus
        {
            get => operationStatus;
            private set
            {
                operationStatus = value;
                Operation?.TriggerStatusChange(this);
            }
        }

        private ModelRenderer[] modelRenderers;
        private OperationStatus operationStatus;


        public ModelExport(GameObject objectToExport, string exportPath)
        {
            ExportPath = exportPath;
            Target = objectToExport;
            OperationStatus = OperationStatus.Waiting;

            IsPrefab = PrefabUtility.IsPartOfAnyPrefab(Target);
            if (IsPrefab)
            {
                PrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Target);
            }
        }

        internal void Success()
        {
            try
            {
                OperationStatus = OperationStatus.Exported;
            }
            catch (System.Exception e)
            {
                Fail();
                Debug.LogException(e);
            }
        }

        internal void Fail()
        {
            OperationStatus = OperationStatus.Failed;
        }

        internal void Apply()
        {
            try
            {
                ApplyMeshes(AssetDatabase.LoadAllAssetRepresentationsAtPath(ExportPath));
                OperationStatus = OperationStatus.Applied;
            }
            catch (Exception e)
            {
                Fail();
                Debug.LogException(e);
            }
        }

        #region static methods

        public static ModelExportOperation Export(GameObject objectToExport, string path) => Export(new ModelExport(objectToExport, path));
        public static ModelExportOperation Export(params ModelExport[] exports) => new ModelExportOperation(exports).Fire();

        internal void InternalExport()
        {
            OperationStatus = OperationStatus.Pending;

            Transform transform = Target.transform;
            ExtractRenderers(transform);

            if (CreateAssets(transform))
                Success();
            else
                Fail();
        }


        private void ExtractRenderers(Transform root)
        {
            List<ModelRenderer> modelRenderers = new();
            List<Transform> transforms = new();
            Renderer[] renderersArray;
            //Going through every lodgroup
            var lodgroups = root.GetComponentsInChildren<LODGroup>(false);
            for (int i = 0; i < lodgroups.Length; i++)
            {
                var lodGroup = lodgroups[i];
                var lods = lodGroup.GetLODs();

                //Going through every lod
                for (int j = 0; j < lodGroup.lodCount; j++)
                {
                    LOD lod = lods[j];
                    renderersArray = lod.renderers;

                    //Going though renderers
                    for (int k = 0; k < renderersArray.Length; k++)
                    {
                        Transform transform = renderersArray[k].transform;

                        if (transforms.Contains(transform))
                            continue;    

                        string uniqueName = $"{root.gameObject.name}_{i}LOD{j}_{k}";
                        transform.gameObject.name = uniqueName;
                        
                        Debug.Log($"{transform.gameObject.name} is renamed in {uniqueName}");
                        transforms.Add(transform);
                        modelRenderers.Add(new ModelRenderer(root, transform, uniqueName));

                    }
                }
            }

            renderersArray = root.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderersArray.Length; i++)
            {
                Transform transform = renderersArray[i].transform;

                if (transforms.Contains(transform.transform))
                    continue;

                string uniqueName = $"{root.gameObject.name}_{i}";
                transform.gameObject.name = uniqueName;

                Debug.Log($"{transform.gameObject.name} is renamed in {uniqueName}");
                transforms.Add(transform);
                modelRenderers.Add(new ModelRenderer(root, transform, uniqueName));
            }

            this.modelRenderers = modelRenderers.ToArray();
        }

        private bool CreateAssets(Transform root)
        {
            GameObject[] gameObjects = new GameObject[modelRenderers.Length];

            for (int i = 0; i < modelRenderers.Length; i++)
                gameObjects[i] = modelRenderers[i].GetTargetGameObject(root);

            ExportPath = UnityEditor.Formats.Fbx.Exporter.ModelExporter.ExportObjects(ExportPath, gameObjects);

            if (string.IsNullOrEmpty(ExportPath))
                return false;

            if (ExportPath.StartsWith(Application.dataPath))
                ExportPath = "Assets" + ExportPath[Application.dataPath.Length..];

            OnModelExported?.Invoke(ExportPath);
            return true;
        }

        private void ApplyMeshes(Object[] objs)
        {
            //If prefab, open an instance that can be modified
            GameObject targetGameObject = PrefabUtility.LoadPrefabContents(PrefabPath);

            List<Mesh> meshesBuffer = new();
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is Mesh mesh)
                    meshesBuffer.Add(mesh);
            }

            Mesh[] meshes = meshesBuffer.ToArray();

            //Reassign meshes
            for (int i = 0; i < modelRenderers.Length; i++)
            {
                ModelRenderer modelRenderer = modelRenderers[i];
                MeshFilter meshFilter = modelRenderer.ApplyMeshToRenderer(targetGameObject.transform, meshes);

                if (meshFilter == null)
                    continue;

                GameObject rendererGO = meshFilter.gameObject;
                if (rendererGO.TryGetComponent(out ProBuilderMesh proBuilderMesh))
                    GameObject.DestroyImmediate(proBuilderMesh, true);

                if (rendererGO.TryGetComponent(out MeshCollider collider))
                    collider.sharedMesh = meshFilter.sharedMesh;

                rendererGO.isStatic = targetGameObject.isStatic;
                rendererGO.layer = targetGameObject.layer;

                if (rendererGO.isStatic)
                {
                    Renderer renderer = rendererGO.GetComponent<Renderer>();
                    renderer.staticShadowCaster = true;
                }
            }
            //If prefab, release this instance and apply changes
            if (IsPrefab)
            {

                PrefabUtility.SaveAsPrefabAsset(targetGameObject, PrefabPath, out bool success);

                if (success)
                    Debug.Log($"Prefab saved at {PrefabPath}");
                
                PrefabUtility.UnloadPrefabContents(targetGameObject);
            }
        }
        #endregion
#endif
    }
}
