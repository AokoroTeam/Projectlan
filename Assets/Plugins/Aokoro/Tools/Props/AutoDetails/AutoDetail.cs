using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Aokoro.Tools.AutoDetails
{
    [System.Serializable]
    public struct AutoDetail
    {
        [ShowAssetPreview, AllowNesting]
        public GameObject source;
        [Range(1,10)]
        public int probability;
    }
}
