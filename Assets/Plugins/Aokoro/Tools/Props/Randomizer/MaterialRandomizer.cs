
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

namespace Aokoro.Tools.Props
{
    
    public class MaterialRandomizer : MonoBehaviour
    {
        public MeshRenderer[] renderers;
        public MaterialRandomizerList data;

        [Button]
        private void Randomize()
        {

            int lenght = renderers.Length;
            Material[] set = new Material[lenght];

            for (int i = 0; i < lenght; i++)
            {
                int rng = Random.Range(0, data.materials.Length);
                set[i] = data.materials[rng];
            }
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sharedMaterials = set;
        }

        [Button]
        private void RandomizeAllWithSameList()
        {
            var props = Object.FindObjectsOfType<MaterialRandomizer>();
            RandomizeMultiple(props.Where(p => p.data == data).ToArray());
        }


        [Button]
        private void RandomizeAll()
        {
            var props = Object.FindObjectsOfType<MaterialRandomizer>();
            RandomizeMultiple(props);
        }


        private void RandomizeMultiple(MaterialRandomizer[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i].Randomize();
        }
    }
}