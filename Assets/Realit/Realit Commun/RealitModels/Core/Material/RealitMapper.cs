using Aokoro.Sequencing;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using Realit.Models.Hierarchy;
using Realit.Models.Meshes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Realit.Models.Materials
{
    [System.Serializable]
    public class RealitMapper
    {
        private const string MaterialsSection = "M";
        private const string TexturesSection = "T";
        private const string RenderersMaterialSection = "RM";

        [SerializeField, ReadOnly,AllowNesting]
        private List<RealitMaterial> realitMaterials;
        [SerializeField, ReadOnly,AllowNesting]
        private List<RealitTexture> realitTextures;

        private Dictionary<int, int[]> renderersMaterials;

        public RealitMapper(RealitMeshData meshData)
        {
            var renderers = meshData.meshRenderers;

            realitMaterials = new List<RealitMaterial>();
            realitTextures = new List<RealitTexture>();
            renderersMaterials = new Dictionary<int, int[]>();


            Dictionary<Material,int> unityMaterialLinks = new();
            Dictionary<Texture, int> unityTextureLinks = new();

            Dictionary<string, int> textures = new();

            //Gathering materials on renderers and converting them
            for (int i = 0; i < renderers.Length; i++)
            {
                var unityMaterials = renderers[i].meshRenderer.sharedMaterials;
                int[] materialsIDs = new int[unityMaterials.Length];

                for (int j = 0; j < unityMaterials.Length; j++)
                {
                    Material mat = unityMaterials[j];
                    
                    //If already exists
                    if (unityMaterialLinks.ContainsKey(mat))
                    {
                        materialsIDs[j] = unityMaterialLinks[mat];
                    }
                    else
                    {
                        ///Extracting Textures
                        //Texture properties
                        var propertyNames = mat.GetTexturePropertyNames();

                        //Skiping maintex
                        for (int pIndex = 1; pIndex < propertyNames.Length; pIndex++)
                        {
                            string propertyName = propertyNames[pIndex];
                            //If texture is valid
                            if (mat.GetTexture(propertyName) is Texture2D t)
                            {
                                if (unityTextureLinks.ContainsKey(t))
                                    textures.Add(propertyName, unityTextureLinks[t]);
                                else
                                {
                                    int texIndex = realitTextures.Count;
                                    textures.Add(propertyName, texIndex);
                                    unityTextureLinks.Add(t, texIndex);

                                    //Creating realit texture
                                    realitTextures.Add(new RealitTexture(t));
                                }
                            }
                        }

                        int matIndex = realitMaterials.Count;

                        materialsIDs[j] = matIndex;
                        RealitMaterial realitMaterial = new RealitMaterial(mat, textures);

                        unityMaterialLinks.Add(mat, matIndex);
                        //materialCrcs.Add(crc, matIndex);
                        realitMaterials.Add(realitMaterial);

                        textures.Clear();
                    }
                }

                renderersMaterials.Add(i, materialsIDs);
            }

        }

        public RealitMapper(JToken json)
        {
            //Converting Json data to realit textures

            var serializedTextures = json[TexturesSection].ToArray();
            int length = serializedTextures.Length;

            realitTextures = new();
            for (int i = 0; i < length; i++)
            {
                RealitTexture realitTexture = new(serializedTextures[i]);
                realitTextures.Add(realitTexture);
            }

            //Converting Json data to materials
            var serializedMaterials = json[MaterialsSection].ToArray();
            length = serializedMaterials.Length;

            realitMaterials = new();
            for (int i = 0; i < length; i++)
            {
                RealitMaterial realitMaterial = new(serializedMaterials[i] as JObject);
                realitMaterials.Add(realitMaterial);
            }

            var serializedRenderersMaterials = json[RenderersMaterialSection].ToArray();
            length = serializedRenderersMaterials.Length;
            renderersMaterials = new();

            for (int i = 0; i < length; i++)
            {
                var ob = serializedRenderersMaterials[i] as JObject;
                int id = (int)ob["id"];
                int[] ids =  ((string)ob["ids"]).Split('/').Select(ctx => int.Parse(ctx)).ToArray();

                renderersMaterials.Add(id, ids);
            }
        }

        public void Map(RealitMeshData meshData)
        {
            RealitMapperContext context = new RealitMapperContext(this);
            var sequence = SequencerBuilder.Begin()
                .Yield()
                .Do(() =>
                {
                    var renderers = meshData.meshRenderers;
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        RealitMeshRenderer realitMeshRenderer = renderers[i];
                        context.GenerateMaterials(realitMeshRenderer, renderersMaterials[i]);
                    }
                })
                .Yield()
                .Do(context.GenerateTextures)
                .Build();

            sequence.Play(SequenceUpdateType.LateUpdate);
        }
        public JToken Serialize()
        {
            int length = realitMaterials.Count;
            JToken[] serializedMaterials = new JToken[length];
            for (int i = 0; i < length; i++)
                serializedMaterials[i] = realitMaterials[i].Serialize();

            //Debug.Log($"{serializedMaterials.Length} Materials");

            length = realitTextures.Count;
            JToken[] serializedTextures = new JToken[length];
            for (int i = 0; i < length; i++)
                serializedTextures[i] = realitTextures[i].Serialize();
            //Debug.Log($"{serializedTextures.Length} Textures");

            List<JObject> serializedRenderersMaterials = new();
            foreach (var kvp in renderersMaterials)
                serializedRenderersMaterials.Add(new JObject(
                    new JProperty("id", kvp.Key),
                    new JProperty("ids", string.Join('/', kvp.Value)))
                    );

            return new JObject(
                    new JProperty(MaterialsSection, serializedMaterials), 
                    new JProperty(TexturesSection, serializedTextures),
                    new JProperty(RenderersMaterialSection, serializedRenderersMaterials)
                );
        }


        #region Utility
        public RealitTexture GetRealitTexture(int id) => realitTextures[id];
        public RealitMaterial GetRealitMaterial(int id) => realitMaterials[id];


        #endregion
    }
}
