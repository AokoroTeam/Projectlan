using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Realit.Models.Hierarchy
{
    [System.Serializable]
    public struct RealitPath
    {
        public int Depth => parentsIds.Length;
        private int[] parentsIds;

        public RealitPath(int[] parentsIds)
        {
            this.parentsIds = parentsIds;
        }


        public static string GetPath(IEnumerable<int> numericPath) => numericPath.Any() ? string.Join('/', numericPath) : string.Empty;

        public static bool IsSubPath(RealitPath path, RealitPath subPath)
        {
            if (path.Depth > subPath.Depth)
                return false;

            var p = path.parentsIds;
            var sp = subPath.parentsIds;

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] != sp[i])
                    return false;
            }

            return true;
        }
    }
}