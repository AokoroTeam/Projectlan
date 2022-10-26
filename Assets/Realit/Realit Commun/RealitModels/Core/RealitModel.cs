using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

using Realit.Models.Hierarchy;
using Realit.Models.Meshes;
using Realit.Models.Materials;
using NaughtyAttributes;

namespace Realit.Models
{
    [System.Serializable]
    public class RealitModel
    {
        private const string HierachySection = "Hierarchy";
        private const string ModelSection = "Model";
        private const string MaterialSection = "Material";

        [AllowNesting]
        public RealitHierachy modelHierachy;
        [AllowNesting]
        public RealitMeshData meshData;
        [AllowNesting]
        public RealitMapper materialMapper;

        public RealitModel(GameObject from)
        { 
            modelHierachy = new RealitHierachy(from);
            meshData = new RealitMeshData(modelHierachy);
            materialMapper = new RealitMapper(meshData);
        }

        public RealitModel(JToken token)
        {
            modelHierachy = new RealitHierachy(token[HierachySection] as JObject);
            meshData = new RealitMeshData(token[ModelSection]);
            materialMapper = new RealitMapper(token[MaterialSection]);
        }

        public GameObject Generate(Transform parent)
        {
            GameObject gameObject = modelHierachy.CreateGameobject(parent);
            
            meshData.PopulateGameObject(modelHierachy);
            materialMapper.Map(meshData);
            
            /*
            RealitModelOptimization.MergeMaterials(modelHierachy.Root.bindedTransform.gameObject);
            RealitModelOptimization.SeparateMesh(modelHierachy.Root.bindedTransform.gameObject);
            */

            return gameObject;
        }

        public JObject Serialize()
        {
            return new JObject(
                    new JProperty(HierachySection, modelHierachy.Serialize()),
                    new JProperty(ModelSection, meshData.Serialize()),
                    new JProperty(MaterialSection, materialMapper.Serialize())
                );
        }

        public override string ToString() => Serialize().ToString(Formatting.None);
    }
}