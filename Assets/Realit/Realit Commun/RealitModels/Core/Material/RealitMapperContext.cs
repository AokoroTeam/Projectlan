using Realit.Models.Meshes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Models.Materials
{
    public class RealitMapperContext
    {
        private RealitMapper mapper;
        public Material defaultMaterial;

        private Dictionary<int, Material> generatedMaterials;
        private Dictionary<int, Texture2D> generatedTextures;

        public RealitMapperContext(RealitMapper mapper)
        {
            this.mapper = mapper;

            generatedMaterials = new();
            generatedTextures = new();
            defaultMaterial = Resources.Load<Material>("Realit/SimpleLitMat");
        }

        public void GenerateMaterials(RealitMeshRenderer realitMeshRenderer, int[] materialIds)
        {
            int length = materialIds.Length;
            Material[] materials = new Material[length];
            for (int i = 0; i < length; i++)
                materials[i] = RequestMaterial(materialIds[i]);
            
            realitMeshRenderer.meshRenderer.sharedMaterials = materials;
        }

        public void GenerateTextures()
        {
            foreach (var kvp in generatedMaterials)
            {
                Material material = kvp.Value;
                RealitMaterial realitMaterial = mapper.GetRealitMaterial(kvp.Key);

                foreach(var txt in realitMaterial.textures)
                    material.SetTexture(txt.Key, RequestTexture(txt.Value));

            }
        }

        private Material RequestMaterial(int id)
        {
            if (generatedMaterials.TryGetValue(id, out Material material))
                return material;
            else
            {
                material = mapper.GetRealitMaterial(id).GenerateMaterial(this);
                generatedMaterials.Add(id, material);
                return material;
            }
        }

        private Texture RequestTexture(int id)
        {
            if (generatedTextures.TryGetValue(id, out Texture2D texture))
                return texture;
            else
            {
                RealitTexture realitTexture = mapper.GetRealitTexture(id);
                realitTexture.GenerateTexture(out texture);

                generatedTextures.Add(id, texture);
                return texture;
            }
        }
    }
}
