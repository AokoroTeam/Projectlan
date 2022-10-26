using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Tools.AutoDetails
{
    [CreateAssetMenu(menuName = "Aokoro/Props/AutoDetails List")]
    public class AutoDetailList : ScriptableObject
    {
        public AutoDetail[] details;
    }
}
