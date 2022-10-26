using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Realit.Models.Hierarchy
{
    [System.Serializable]
    public class RealitHierachy
    {
        public RealitTransform Root => transforms[0];
        public int TransformCount => transforms.Length;

        [SerializeField, ReadOnly]
        private RealitTransform[] transforms;
        [SerializeField, ReadOnly]
        private RealitPath[] transformsPaths;

        private Dictionary<int, int> transformsParents;
        
        public RealitHierachy(GameObject from)
        {
            transformsParents = new Dictionary<int, int>();
            List<RealitTransform> tList = new List<RealitTransform>();
            RegisterExistingTransform(from.transform, tList);

            transforms = tList.ToArray();
            GeneratePaths();
        }

        public RealitHierachy(JObject json)
        {
            transformsParents = new Dictionary<int, int>();
            List<RealitTransform> transformList = new List<RealitTransform>();
            //Root
            RealitTransform root = new RealitTransform(json, 0);
            transformList.Add(root);

            CreateChildren(json, root, transformList);
            transforms = transformList.ToArray();

            GeneratePaths();
        }

        public GameObject CreateGameobject(Transform parent)
        {
            Dictionary<RealitTransform, Transform> createdTransforms = transforms.ToDictionary(t => t, t => t.CreateTransform());
            foreach (var kvp in createdTransforms)
            {
                RealitTransform rmt = kvp.Key;
                Transform t = kvp.Value;
                
                if (!IsRoot(rmt))
                {
                    RealitTransform p = GetParent(rmt);
                    t.SetParent(createdTransforms[p], false);
                }
            }

            Transform root = createdTransforms[transforms[0]];
            root.SetParent(parent, false);

            return root.gameObject;
        }

        private void CreateChildren(JObject rmtJson, RealitTransform parent, List<RealitTransform> existingTransforms)
        {
            if (rmtJson.TryGetValue("children", out JToken token))
            {
                foreach (JObject childJson in token)
                {
                    int index = existingTransforms.Count;
                    var rmt = new RealitTransform(childJson, index);

                    existingTransforms.Add(rmt);
                    transformsParents.Add(index, parent.ID);

                    CreateChildren(childJson, rmt, existingTransforms);
                }
            }
        }

        private void GeneratePaths()
        {
            int length = transforms.Length;
            transformsPaths = new RealitPath[length];

            for (int i = 0; i < transforms.Length; i++)
            {
                RealitTransform t = transforms[i];
                bool pathCompleted = false;

                int head = t.ID;
                List<int> ids = new() { head };

                while(!pathCompleted)
                {
                    if(transformsParents.TryGetValue(head, out int parentId))
                    {
                        ids.Add(parentId);
                        head = parentId;
                    }
                    else
                        pathCompleted = true;
                }

                ids.Reverse();
                transformsPaths[i] = new RealitPath(ids.ToArray());
            }
        }
        private void RegisterExistingTransform(Transform transform, List<RealitTransform> list, int parent = -1)
        {
            int parentIndex = list.Count;
            RealitTransform t = new RealitTransform(transform, parentIndex);
            list.Add(t);

            if(parent != -1)
                transformsParents.Add(parentIndex, parent);

            for (int i = 0; i < transform.childCount; i++)
            {
                //Goes through all children
                RegisterExistingTransform(transform.GetChild(i), list, parentIndex);
            }
        }

        #region Utility

        public RealitTransform GetTransform(int ID) => transforms[ID];
        public bool IsRoot(RealitTransform transform) => !transformsParents.ContainsKey(transform.ID);
        public bool GetParent(RealitTransform transform, out RealitTransform parent)
        {
            parent = default;

            if (IsRoot(transform))
                return false;

            parent = transforms[transformsParents[transform.ID]];
            return true;
        }
        public RealitPath GetPath(RealitTransform transform) => transformsPaths[transform.ID];
        public RealitTransform GetParent(RealitTransform transform) => transformsParents.TryGetValue(transform.ID, out int parent) ? transforms[parent] : default;
        #endregion

        public JToken Serialize() => GetObjectWithChildrens(transforms[0]);

        private JObject GetObjectWithChildrens(RealitTransform transform)
        {
            JObject parent = transform.Serialize() as JObject;
            IEnumerable<JObject> children = transformsParents
                .Where(ctx => ctx.Value == transform.ID)
                .Select(ctx => GetObjectWithChildrens(transforms[ctx.Key]));

            if (children.Any())
                parent.Add(new JProperty("children", children));

            return parent;
        }

        
    }
}