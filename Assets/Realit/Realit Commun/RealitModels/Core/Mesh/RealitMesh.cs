using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Mesh;
using Unity.Collections;
using ReadOnlyAttribute = NaughtyAttributes.ReadOnlyAttribute;
using Newtonsoft.Json;

namespace Realit.Models.Meshes
{
    [System.Serializable]
    public struct RealitMesh
    {
        private const string VerticesKey = "vertices";
        private const string NormalKey = "normals";
        private const string SubMeshesKey = "submeshes";
        private const string UV1Key = "uv1";
        private const string UV2Key = "uv2";
        private const string UV3Key = "uv3";
        private const string UV4Key = "uv4";

        public int VerticesCount => Positions.Length;
        public Vector3[] Positions { get; private set; }
        
        public Vector3[] Normals { get; private set; }

        public Vector2[] Uv1 { get; private set; }        
        public Vector2[] Uv2 { get; private set; }
        public Vector2[] Uv3 { get; private set; }
        public Vector2[] Uv4 { get; private set; }
        public RealitModelSubmesh[] Submeshes { get; private set; }

        public RealitMesh(MeshData meshData)
        {
            //Vertices
            using (var gotVertices = new NativeArray<Vector3>(meshData.vertexCount, Allocator.Temp))
            {
                meshData.GetVertices(gotVertices);
                Positions = new Vector3[gotVertices.Length];
                gotVertices.CopyTo(Positions);
            }

            //Normal
            using (var gotNormals = new NativeArray<Vector3>(meshData.vertexCount, Allocator.Temp))
            {
                meshData.GetNormals(gotNormals);
                Normals = new Vector3[gotNormals.Length];
                gotNormals.CopyTo(Normals);
            }

            //Triangles
            int subMeshCount = meshData.subMeshCount;
            Submeshes = new RealitModelSubmesh[subMeshCount];

            for (int i = 0; i < subMeshCount; i++)
            {
                SubMeshDescriptor subMesh = meshData.GetSubMesh(i);
                using (var gotIndices = new NativeArray<int>(subMesh.indexCount, Allocator.Temp))
                {
                    meshData.GetIndices(gotIndices, i, true);
                    Submeshes[i] = new RealitModelSubmesh(gotIndices.ToArray(), i, subMesh.topology);
                }
            }

            if (meshData.HasVertexAttribute(VertexAttribute.TexCoord0))
            {
                using (var uvData = new NativeArray<Vector2>(meshData.vertexCount, Allocator.Temp))
                {
                    meshData.GetUVs(0, uvData);
                    Uv1 = uvData.ToArray();
                }
            }
            else
                Uv1 = null;

            if (meshData.HasVertexAttribute(VertexAttribute.TexCoord1))
            {
                using (var uvData = new NativeArray<Vector2>(meshData.vertexCount, Allocator.Temp))
                {
                    meshData.GetUVs(1, uvData);
                    Uv2 = uvData.ToArray();
                }
            }
            else
                Uv2 = null;

            if (meshData.HasVertexAttribute(VertexAttribute.TexCoord2))
            {
                using (var uvData = new NativeArray<Vector2>(meshData.vertexCount, Allocator.Temp))
                {
                    meshData.GetUVs(2, uvData);
                    Uv3 = uvData.ToArray();
                }
            }
            else
                Uv3 = null;

            if (meshData.HasVertexAttribute(VertexAttribute.TexCoord3))
            {
                using (var uvData = new NativeArray<Vector2>(meshData.vertexCount, Allocator.Temp))
                {
                    meshData.GetUVs(3, uvData);
                    Uv4 = uvData.ToArray();
                }
            }
            else
                Uv4 = null;
        }

        public RealitMesh(JObject jObj)
        {
            Positions = new Vector3[0].Deserialize(jObj[VerticesKey]);
            Normals = new Vector3[0].Deserialize(jObj[NormalKey]);

            Uv1 = new Vector2[0].Deserialize(jObj[UV1Key]);
            Uv2 = jObj.TryGetValue(UV2Key, out JToken uvToken) ? new Vector2[0].Deserialize(uvToken) : null;
            Uv3 = jObj.TryGetValue(UV3Key, out uvToken) ? new Vector2[0].Deserialize(uvToken) : null;
            Uv4 = jObj.TryGetValue(UV4Key, out uvToken) ? new Vector2[0].Deserialize(uvToken) : null;

            JToken submeshesToken = jObj[SubMeshesKey];
            Submeshes = submeshesToken
                .Select((submeshToken, index) =>
                {
                    MeshTopology topology = (MeshTopology)submeshToken[RealitModelSubmesh.TopologyKey].ToObject<int>();
                    int[] indices = submeshToken[RealitModelSubmesh.IndicesKey].ToObject<int[]>();
                    return new RealitModelSubmesh(indices, index, topology);
                })
                .ToArray();
        }

        public MeshData PopulateMeshData(MeshData meshData)
        {
            //Vertex
            int verticeCount = Positions.Length;

            VertexAttributeDescriptor[] attributes = RealitModelVertex.GetDescriptors();
            meshData.SetVertexBufferParams(verticeCount, attributes);
            NativeArray<RealitModelVertex> vertexData = meshData.GetVertexData<RealitModelVertex>();

            for (int i = 0; i < vertexData.Length; i++)
                vertexData[i] = new RealitModelVertex(Positions, Normals, Uv1, Uv2, Uv3, Uv4, i);

            var indices = Submeshes.SelectMany(submesh => submesh.indices);
            meshData.SetIndexBufferParams(indices.Count(), IndexFormat.UInt32);

            NativeArray<int> indexData = meshData.GetIndexData<int>();
            indexData.CopyFrom(indices.ToArray());

            // One sub-mesh with all the indices.
            int submeshCount = Submeshes.Length;
            int last = 0;

            meshData.subMeshCount = submeshCount;
            for (int i = 0; i < submeshCount; i++)
            {
                RealitModelSubmesh realitModelSubmesh = Submeshes[i];
                int length = realitModelSubmesh.indices.Length;
                meshData.SetSubMesh(i, new SubMeshDescriptor(last, length, realitModelSubmesh.topology));
                last += length;
            }

            return meshData;
        }

        public void GetVertices(RealitModelVertex[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = new RealitModelVertex(Positions, Normals, Uv1, Uv2, Uv3, Uv3, i);
        }
        public RealitModelVertex GetVertex(int index) => new(Positions, Normals, Uv1, Uv2, Uv3, Uv3, index);

        public JToken Serialize()
        {
            JArray submeshArray = new();
            //Submeshes
            for (int i = 0; i < Submeshes.Length; i++)
            {
                RealitModelSubmesh realitModelSubmesh = Submeshes[i];
                submeshArray.Add(new JObject(
                    new JProperty(RealitModelSubmesh.TopologyKey, (int)realitModelSubmesh.topology),
                    new JProperty(RealitModelSubmesh.IndicesKey, new JArray(realitModelSubmesh.indices)))
                    );
            }

            JObject obj = new()
            {
                { VerticesKey, Positions.Serialize() },
                { NormalKey, Normals.Serialize() },
                { SubMeshesKey, submeshArray },
                { UV1Key, Uv1.Serialize() }
            };

            //Optionnal UVs
            if (Uv2 != null)
                obj.Add(UV2Key, Uv2.Serialize());
            if (Uv3 != null)
                obj.Add(UV3Key, Uv3.Serialize());
            if (Uv4 != null)
                obj.Add(UV4Key, Uv4.Serialize());

            return obj;
        }

    }
}