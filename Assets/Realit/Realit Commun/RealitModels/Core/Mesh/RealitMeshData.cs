using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;


using Realit.Models.Hierarchy;

namespace Realit.Models.Meshes
{
    [System.Serializable]
    public class RealitMeshData
    {
        private const string meshSection = "meshes";
        private const string renderersSection = "renderers";
        [ReadOnly]
        public RealitMesh[] meshes;
        [ReadOnly]
        public RealitMeshRenderer[] meshRenderers;

        public RealitMeshData(RealitHierachy realitModelHierachy)
        {
            ExtractMeshesFromHierachy(realitModelHierachy);
        }

        public RealitMeshData(JToken jToken)
        {
            //Creating meshes
            var meshesArray = jToken[meshSection].ToArray();
            int meshCount = meshesArray.Length;
            meshes = new RealitMesh[meshCount];
            
            for (int i = 0; i < meshCount; i++)
                meshes[i] = new RealitMesh(meshesArray[i] as JObject);

            //Creating meshes
            var renderersArray = jToken[renderersSection].ToArray();
            int rendererCount = renderersArray.Length;
            meshRenderers = new RealitMeshRenderer[rendererCount];

            for (int i = 0; i < rendererCount; i++)
                meshRenderers[i] = new RealitMeshRenderer(renderersArray[i]);
        }

        private void ExtractMeshesFromHierachy(RealitHierachy realitModelHierachy)
        {
            //First, extract mesh
            List<Mesh> meshObjects = new List<Mesh>();
            List<RealitMeshRenderer> renderers = new List<RealitMeshRenderer>();
            
            MeshFilter meshFilter;
            MeshRenderer renderer;
            Mesh mesh;

            for (int i = 0; i < realitModelHierachy.TransformCount; i++)
            {
                var rmt = realitModelHierachy.GetTransform(i);
                if (rmt.bindedTransform.TryGetComponent(out meshFilter))
                {
                    mesh = meshFilter.sharedMesh;
                    if (mesh.vertexCount == 0)
                        continue;

                    if (!meshObjects.Contains(mesh))
                        meshObjects.Add(mesh);

                    if (meshFilter.TryGetComponent(out renderer))
                        renderers.Add(new RealitMeshRenderer(i, meshObjects.IndexOf(mesh), renderer));
                }
            }

            meshRenderers = renderers.ToArray();
            meshes = new RealitMesh[meshObjects.Count];
            using (Mesh.MeshDataArray meshesData = Mesh.AcquireReadOnlyMeshData(meshObjects))
            {
                for (int i = 0; i < meshObjects.Count; i++)
                    meshes[i] = new RealitMesh(meshesData[i]);
            }
        }

        public void PopulateGameObject(RealitHierachy modelHierachy)
        {
            int meshCount = meshes.Length;
            Mesh[] generatedMeshes = new Mesh[meshCount];
            //List<MeshRenderer> renderers = new List<MeshRenderer>();

            GenerateMeshes(generatedMeshes);

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                var realitRenderer = meshRenderers[i];
                Mesh mesh = generatedMeshes[realitRenderer.meshID];
                MeshRenderer meshRenderer = realitRenderer.GenerateMeshRenderer(modelHierachy);

                GameObject go = meshRenderer.gameObject;
                MeshFilter meshFilter = go.AddComponent<MeshFilter>();

                meshFilter.mesh = mesh;
                meshRenderer.materials = new Material[mesh.subMeshCount];

                meshRenderers[i] = realitRenderer;

                MeshCollider meshCollider = go.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
                meshCollider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation 
                    | MeshColliderCookingOptions.WeldColocatedVertices 
                    | MeshColliderCookingOptions.UseFastMidphase;

                go.layer = LayerMask.NameToLayer("Ground");
            }

            
        }

        

        private void GenerateMeshes(Mesh[] generatedMeshBuffers)
        {
            // Allocate mesh data for one mesh.
            int length = generatedMeshBuffers.Length;
            var dataArray = Mesh.AllocateWritableMeshData(length);
            
            try
            {
                for (int i = 0; i < length; i++)
                {
                    var realitMeshData = meshes[i];
                    var data = dataArray[i];

                    realitMeshData.PopulateMeshData(data);
                    generatedMeshBuffers[i] = new Mesh();
                    generatedMeshBuffers[i].name = i.ToString();
                }

                Mesh.ApplyAndDisposeWritableMeshData(dataArray, generatedMeshBuffers);

                for (int i = 0; i < length; i++)
                {
                    var mesh = generatedMeshBuffers[i];
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mesh.Optimize();
                }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                dataArray.Dispose();
                throw;
            }
        }

        public JObject Serialize()
        {
            int meshCount = meshes.Length;
            JToken[] meshTokens = new JToken[meshCount];
            for (int i = 0; i < meshCount; i++)
                meshTokens[i] = meshes[i].Serialize();


            int rendererCount = meshRenderers.Length;
            JToken[] renderersTokens = new JToken[meshCount];

            for (int i = 0; i < meshCount; i++)
                renderersTokens[i] = meshRenderers[i].Serialize();

            return new JObject(
                new JProperty(meshSection, meshTokens),
                new JProperty(renderersSection, renderersTokens));
        }
    }
}