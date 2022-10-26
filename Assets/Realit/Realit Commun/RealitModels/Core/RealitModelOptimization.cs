using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TriLibCore.General;
using UnityEngine.Experimental.Rendering;
using Unity.Collections;
using System.Linq;
using UnityEngine.Rendering;
using Realit.Models.Meshes;
using static UnityEngine.Mesh;
using UnityEngine.UIElements;
using UnityEngine.AI;
using TriLibCore.Interfaces;

namespace Realit.Models
{
    public static class RealitModelOptimization
    {
        public static GameObject Split(this MeshRenderer meshRenderer, params int[] submeshes)
        {
            if (!meshRenderer.TryGetComponent(out MeshFilter cuttedMeshFilter))
                return null;

            return InternalSplit(meshRenderer, cuttedMeshFilter, submeshes);
        }
        public static GameObject Split(this MeshFilter meshFilter, params int[] submeshes)
        {
            if (!meshFilter.TryGetComponent(out MeshRenderer meshRenderer))
                return null;

            return InternalSplit(meshRenderer, meshFilter, submeshes);
        }

        private static GameObject InternalSplit(MeshRenderer cuttedMeshRenderer, MeshFilter cuttedMeshFilter, int[] submeshes)
        {
            Mesh originalMesh = Application.isPlaying ? cuttedMeshFilter.mesh : cuttedMeshFilter.sharedMesh;
            int originalSubMeshCount = originalMesh.subMeshCount;

            //Generating the affected mesh
            int[] cuttedSubmeshes = Enumerable.Range(0, originalSubMeshCount).Except(submeshes).ToArray();
            Mesh cuttedMesh = originalMesh.ExtractSubMeshes(cuttedSubmeshes);

            //Generating the produced mesh
            Mesh producedMesh = originalMesh.ExtractSubMeshes(submeshes);

            //Creating the Gameobject
            GameObject producedGO = new GameObject($"{cuttedMeshRenderer.gameObject.name}_Splited");
            Transform producedTransform = producedGO.transform;
            producedTransform.parent = producedTransform.parent;
            producedTransform.SetPositionAndRotation(cuttedMeshRenderer.transform.position, cuttedMeshRenderer.transform.rotation);
            producedTransform.localScale = cuttedMeshRenderer.transform.localScale;

            MeshRenderer producedMeshRenderer = producedGO.AddComponent<MeshRenderer>();
            MeshFilter producedMeshFilter = producedGO.AddComponent<MeshFilter>();

            //Assinging meshes
            cuttedMeshFilter.sharedMesh = cuttedMesh;
            producedMeshFilter.sharedMesh = producedMesh;

            //Removing extracted Materials
            var mats = new List<Material>(cuttedMeshRenderer.sharedMaterials);
            var transferedMats = mats.Where(ctx => submeshes.Contains(mats.IndexOf(ctx)));

            cuttedMeshRenderer.sharedMaterials = mats.Except(transferedMats).ToArray();
            producedMeshRenderer.sharedMaterials = transferedMats.ToArray();

            return producedGO;
        }

        public static void Optimize(this MeshRenderer meshRenderer, Dictionary<Hash128, Material> existingMaterials = null)
        {

            if (!meshRenderer.TryGetComponent(out MeshFilter meshFilter))
                return;

            InternalOptimize(meshRenderer, meshFilter, existingMaterials);
        }
        public static void Optimize(this MeshFilter meshFilter, Dictionary<Hash128, Material> existingMaterials = null)
        {

            if (!meshFilter.TryGetComponent(out MeshRenderer meshRenderer))
                return;

            InternalOptimize(meshRenderer, meshFilter, existingMaterials);
        }
        private static void InternalOptimize(MeshRenderer meshRenderer, MeshFilter meshFilter, Dictionary<Hash128, Material> existingMaterials = null)
        {
            var uniqueMaterials = meshRenderer.OptimizeMaterials(existingMaterials);
            var mats = meshRenderer.sharedMaterials;

            var mergeableSubmeshes = new Dictionary<int, List<int>>();

            for (int i = 0; i < uniqueMaterials.Length; i++)
            {
                Material mat = uniqueMaterials[i];
                mergeableSubmeshes.Add(i, new List<int>());

                for (int j = 0; j < mats.Length; j++)
                {
                    Material secondMat = mats[j];
                    if (secondMat == mat)
                        mergeableSubmeshes[i].Add(j);
                }
            }

            Mesh mesh = meshFilter.sharedMesh;
            bool needSubmeshMerge = mergeableSubmeshes.Any(ctx => ctx.Value.Count > 1);
            if (needSubmeshMerge)
            {
                RealitMesh realitMesh;
                int newSubmeshCount = uniqueMaterials.Length;
                var newSubmeshes = new SubMeshDescriptor[newSubmeshCount];
                var newIndices = new List<int>();

                //Rearranging submeshes
                using (var readableMeshDataArray = Mesh.AcquireReadOnlyMeshData(mesh))
                {
                    realitMesh = new RealitMesh(readableMeshDataArray[0]);

                    for (int i = 0; i < newSubmeshCount; i++)
                    {
                        List<int> toMerge = mergeableSubmeshes[i];
                        int indexStart = newIndices.Count;

                        foreach (var submeshIndex in toMerge)
                        {
                            RealitModelSubmesh submesh = realitMesh.Submeshes[submeshIndex];
                            int[] indices = submesh.indices;
                            int length = indices.Length;

                            for (int k = 0; k < length; k++)
                                newIndices.Add(indices[k]);

                        }

                        newSubmeshes[i] = new SubMeshDescriptor(indexStart, newIndices.Count - indexStart, MeshTopology.Triangles);
                    }
                }
                var vertices = new RealitModelVertex[realitMesh.VerticesCount];
                realitMesh.GetVertices(vertices);

                ApplyChangesToMesh(mesh, newSubmeshes,vertices, newIndices.ToArray());
            }

            mesh.Optimize();
            mesh.OptimizeIndexBuffers();
            mesh.OptimizeReorderVertexBuffer();

            meshRenderer.sharedMaterials = uniqueMaterials;
            meshFilter.sharedMesh = mesh;
        }

        public static Mesh ExtractSubMeshes(this Mesh mesh, params int[] submeshIndices)
        {
            int newSubmeshCount = submeshIndices.Length;

            var newVertices = new List<RealitModelVertex>();
            
            var newSubmeshes = new SubMeshDescriptor[newSubmeshCount];
            var newIndices = new List<int>();

            using (var readableMeshDataArray = Mesh.AcquireReadOnlyMeshData(mesh))
            {
                RealitMesh realitMesh = new RealitMesh(readableMeshDataArray[0]);
                var indicesLink = new Dictionary<int, int>();

                //Getting indices 
                int firstIndex = 0;
                for (int i = 0; i < newSubmeshCount; i++)
                {
                    int submeshIndex = submeshIndices[i];
                    RealitModelSubmesh submesh = realitMesh.Submeshes[submeshIndex];
                    int[] indices = submesh.indices;
                    int length = indices.Length;

                    for (int j = 0; j < length; j++)
                    {
                        int oldIndex = indices[j];
                        if (!indicesLink.TryGetValue(oldIndex, out int newIndex))
                        {
                            newIndex = newVertices.Count;
                            //Link between old and new indices
                            indicesLink.Add(oldIndex, newIndex);
                            //Registering vertex
                            newVertices.Add(realitMesh.GetVertex(oldIndex));
                        }

                        newIndices.Add(newIndex);
                    }

                    newSubmeshes[i] = new SubMeshDescriptor(firstIndex, length, submesh.topology);
                    firstIndex += length;
                }
            }
            Mesh newMesh = new Mesh();
            ApplyChangesToMesh(newMesh, newSubmeshes, newVertices.ToArray(), newIndices.ToArray());
            return newMesh;
        }

        private static void ApplyChangesToMesh(Mesh mesh, SubMeshDescriptor[] submeshes, RealitModelVertex[] vertices, int[] indices)
        {
            var writableMeshDataArray = Mesh.AllocateWritableMeshData(1);
            //Creating mesh with jobs
            try
            {
                MeshData meshData = writableMeshDataArray[0];

                //Index buffer and data
                IndexFormat format = indices.Length > 65536 ? IndexFormat.UInt32 : IndexFormat.UInt16;
                meshData.SetIndexBufferParams(indices.Length, format);
                if (format == IndexFormat.UInt32)
                {
                    NativeArray<int> indexData = meshData.GetIndexData<int>();
                    for (int i = 0; i < indexData.Length; i++)
                        indexData[i] = indices[i];
                }
                else
                {
                    NativeArray<ushort> indexData = meshData.GetIndexData<ushort>();
                    for (int i = 0; i < indexData.Length; i++)
                        indexData[i] = (ushort)indices[i];
                }

                //Vertices buffer and data
                int verticesCount = vertices.Length;
                meshData.SetVertexBufferParams(verticesCount, RealitModelVertex.GetDescriptors());
                NativeArray<RealitModelVertex> vertexData = meshData.GetVertexData<RealitModelVertex>();

                for (int i = 0; i < verticesCount; i++)
                    vertexData[i] = vertices[i];

                //Submeshes (Always after setting up vertices)
                int submeshCount = submeshes.Length;
                meshData.subMeshCount = submeshCount;
                for (int i = 0; i < submeshCount; i++)
                    meshData.SetSubMesh(i, submeshes[i], MeshUpdateFlags.Default);

                //Applying and disposing
                Mesh.ApplyAndDisposeWritableMeshData(writableMeshDataArray, mesh, MeshUpdateFlags.Default);
            }
            catch (System.Exception e)
            {
                writableMeshDataArray.Dispose();
                Debug.LogException(e);
            }
        }
        #region Materials


        public static Material[] OptimizeMaterials(this MeshRenderer renderer, Dictionary<Hash128, Material> sameMaterials = null)
        {
            if(sameMaterials == null)
                sameMaterials = new Dictionary<Hash128, Material>();

            Material[] materials = renderer.sharedMaterials;

            for (int j = 0; j < materials.Length; j++)
            {
                Material material = materials[j];
                Hash128 crc = GetMaterialHash(material);

                if (sameMaterials.TryGetValue(crc, out Material exisiting))
                {
                    //AssetDatabase.DeleteAsset(materialAssetPath);
                    materials[j] = exisiting;
                }
                else
                    sameMaterials.Add(crc, material);
            }

            renderer.sharedMaterials = materials;

            return sameMaterials.Values.ToArray();
        }

        public static Hash128 GetMaterialHash(Material material)
        {
            var hashes = new List<Hash128>();
            Color color = material.color;

            hashes.Add(Hash128.Compute(new float[] { color.r, color.g, color.b, color.a }));
            hashes.Add(Hash128.Compute(material.GetFloat("_Metallic")));
            hashes.Add(Hash128.Compute(material.GetFloat("_Smoothness")));

            Texture2D mainTexture = material.mainTexture as Texture2D;
            if (mainTexture != null)
            {
                // Create a temporary RenderTexture of the same size as the texture
                RenderTexture tmp = RenderTexture.GetTemporary(65, 65);

                // Blit the pixels on texture to the RenderTexture
                Graphics.Blit(mainTexture, tmp);
                // Backup the currently set RenderTexture
                RenderTexture previous = RenderTexture.active;
                // Set the current RenderTexture to the temporary one we created
                RenderTexture.active = tmp;
                // Create a new readable Texture2D to copy the pixels to it
                Texture2D hashedTexture = new Texture2D(65, 65);
                // Copy the pixels from the RenderTexture to the new Texture
                hashedTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                hashedTexture.Apply();
                // Reset the active RenderTexture
                RenderTexture.active = previous;
                // Release the temporary RenderTexture
                RenderTexture.ReleaseTemporary(tmp);

                using (var data = hashedTexture.GetPixelData<byte>(0))
                {
                    Hash128 tHash = GetTextureHash(data);
                    //Debug.Log($"[Model Optimisation] TextureHash for {mainTexture.name} is {tHash}");
                    hashes.Add(tHash);
                }

            }

            Hash128 hash = Hash128.Compute(hashes);
            //Debug.Log($"[Model Optimisation] Hash for {material.name} is {hash}");
            return hash;
        }

        public static Hash128 GetTextureHash(NativeArray<byte> data)
        {
            int lenght = data.Length;
            int[] textureHashContent = new int[lenght];

            for (int i = 0; i < lenght; i++)
                textureHashContent[i] = data[i];

            Hash128 hash128 = Hash128.Compute(textureHashContent);
            return hash128;
        }
        #endregion
    }
}