using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


namespace Realit.Models.Meshes
{
    [System.Serializable]
    public struct RealitModelSubmesh
    {
        public const string TopologyKey = "topology";
        public const string IndicesKey = "indices";

        [JsonIgnore, ShowNonSerializedField]
        public int submeshIndex;

        [ReadOnly]
        public MeshTopology topology;
        [ReadOnly]
        public int[] indices;

        public RealitModelSubmesh(int[] indices, int submeshIndex, MeshTopology topology)
        {
            this.submeshIndex = submeshIndex;
            this.indices = indices;
            this.topology = topology;
        }
    }
}