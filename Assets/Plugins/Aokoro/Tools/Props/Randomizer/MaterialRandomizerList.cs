using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Tools.Props
{
    [CreateAssetMenu(menuName = "Aokoro/Props/Material Randomizer List")]
    public class MaterialRandomizerList : ScriptableObject
    {
        public Material[] materials;
    }
}