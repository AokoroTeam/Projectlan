using Newtonsoft.Json.Linq;
using Realit.Models.Hierarchy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Mesh;

namespace Realit.Models.Meshes
{

    [System.Serializable]
    public struct RealitMeshRenderer
    {
        private const string transformKey = "t";
        private const string meshKey = "m";
        private const string materialsKey = "materials";

        public MeshRenderer meshRenderer;

        public int transformID;
        public int meshID;

        public RealitMeshRenderer(int transformID, int meshID, MeshRenderer meshRenderer)
        {
            this.transformID = transformID;
            this.meshID = meshID;
            this.meshRenderer = meshRenderer;

        }
        public RealitMeshRenderer(JToken token)
        {
            this.transformID = ((int)token[transformKey]);
            this.meshID = ((int)token[meshKey]);

            meshRenderer = null;
        }

        public MeshRenderer GenerateMeshRenderer(RealitHierachy hierachy)
        {
            var t = hierachy.GetTransform(transformID).bindedTransform;
            meshRenderer = t.gameObject.AddComponent<MeshRenderer>();
            return meshRenderer;
        }

        public JToken Serialize() => new JObject(
            new JProperty(transformKey, transformID),
            new JProperty(meshKey, meshID));
    }
}