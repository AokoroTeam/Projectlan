using Aokoro.Sequencing;
using NaughtyAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Realit.Models.Materials
{

    [System.Serializable]
    public class RealitMaterial
    {
        private const string NameKey = "name";
        private const string ShaderKey = "shader";
        private const string RenderQueueKey = "renderQueue";
        private const string KeywordsKey = "keywords";
        private const string FloatsKey = "floats";
        private const string ColorsKey = "colors";
        private const string TexturesKey = "textures";

        [SerializeField, ReadOnly]
        private string[] keywords;
        [SerializeField, ReadOnly]
        private string name;
        [SerializeField, ReadOnly]
        private int renderQueue;
        [SerializeField, ReadOnly]
        private Material mat;
        [SerializeField, ReadOnly]
        private string shaderName;

        public Dictionary<string, float> floats = new()
        {
            {"_AlphaClip", -1},
            {"_Blend", -1},
            {"_BumpScale", -1},
            {"_ClearCoatMask", -1},
            {"_ClearCoatSmoothness", -1},
            {"_Cull", -1},
            {"_Cutoff", -1},
            {"_DetailAlbedoMapScale", -1},
            {"_DetailNormalMapScale", -1},
            {"_DstBlend", -1},
            {"_EnvironmentReflections", -1},
            {"_GlossMapScale", -1},
            {"_Glossiness", -1},
            {"_GlossyReflections", -1},
            {"_Metallic", -1},
            {"_Mode", -1},
            {"_OcclusionStrength", -1},
            {"_Parallax", -1},
            {"_QueueOffset", -1},
            {"_ReceiveShadows", -1},
            {"_Smoothness", -1},
            {"_SmoothnessTextureChannel", -1},
            {"_SpecularHighlights", -1},
            {"_SrcBlend", -1},
            {"_Surface", -1},
            {"_UVSec", -1},
            {"_WorkflowMode", -1},
            {"_ZWrite", -1},
        };

        public Dictionary<string, string> colors = new()
        {
            {"_BaseColor", ""},
            {"_Color", ""},
            {"_EmissionColor", ""},
            {"_SpecColorColor", ""},
        }; 
        
        
        public Dictionary<string, int> textures = new(){};

        public RealitMaterial(Material material, Dictionary<string, int> textures)
        {
            this.mat = material;
            this.name = material.name;
            this.shaderName = material.shader.name;
            this.renderQueue = material.renderQueue;

            this.textures = new Dictionary<string, int>(textures);

            var floats = this.floats.Keys.ToArray();
            foreach(var f in floats)
            {
                if (material.HasFloat(f))
                    this.floats[f] = material.GetFloat(f);
            }

            var colors = this.colors.Keys.ToArray();
            foreach (var c in colors)
            {
                if (material.HasColor(c))
                    this.colors[c] = ColorUtility.ToHtmlStringRGBA(material.GetColor(c));
            }

            keywords = material.shaderKeywords;
        }

        public RealitMaterial(JObject json)
        {
            if (json.TryGetValue(NameKey, out JToken token))
                name = (string)token;
            
            if (json.TryGetValue(ShaderKey, out token))
                shaderName = (string)token;

            if (json.TryGetValue(RenderQueueKey, out token))
                renderQueue = token.ToObject<int>();

            if (json.TryGetValue(KeywordsKey, out token))
                keywords = token.ToObject<string[]>();
            
            if (json.TryGetValue(ColorsKey, out token))
                colors = token.ToObject<Dictionary<string, string>>();
            
            if (json.TryGetValue(FloatsKey, out token))
                floats = token.ToObject<Dictionary<string, float>>();

            if (json.TryGetValue(TexturesKey, out token))
                textures = token.ToObject<Dictionary<string, int>>();

            mat = null;
        }

        public Material GenerateMaterial(RealitMapperContext context)
        {
            Shader shader = Shader.Find(shaderName);
            mat = new Material(shader);
            mat.name = name;
            mat.renderQueue = renderQueue;

            foreach (var kvp in floats)
            {
                if(mat.HasFloat(kvp.Key))
                    mat.SetFloat(kvp.Key, kvp.Value);
            }

            foreach (var kvp in colors)
            {
                if (mat.HasColor(kvp.Key))
                {
                    if(!ColorUtility.TryParseHtmlString(kvp.Value, out Color color))
                        color = Color.white;

                    mat.SetColor(kvp.Key, color);
                }
            }

            mat.shaderKeywords = keywords;

            for (int i = 0; i < keywords.Length; i++)
                mat.EnableKeyword(keywords[i]);

            SequencerBuilder.Begin()
                .WaitForFrames(5)
                .Do(() => mat.SetFloat("_Smoothness", 0))
                .Build().Play();
            return mat;
        }



        public JToken Serialize()
        {
            return new JObject(
                    new JProperty(NameKey, name),
                    new JProperty(ShaderKey, shaderName),
                    new JProperty(RenderQueueKey, renderQueue),
                    new JProperty(KeywordsKey, keywords),
                    new JProperty(FloatsKey, JObject.Parse(JsonConvert.SerializeObject(floats))),
                    new JProperty(ColorsKey, JObject.Parse(JsonConvert.SerializeObject(colors))),
                    new JProperty(TexturesKey, JObject.Parse(JsonConvert.SerializeObject(textures)))
                );
        }
    }
}