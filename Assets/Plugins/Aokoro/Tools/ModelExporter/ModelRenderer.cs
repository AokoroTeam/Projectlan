using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.ModelExports
{
    public class ModelRenderer
    {
#if UNITY_EDITOR
        public int[] meshFilterPath;
        public string meshName;

        public ModelRenderer(Transform root, Transform transform, string meshName)
        {
            this.meshName = meshName;
            if (transform == root)
                meshFilterPath = null;
            else
            {
                List<int> indices = new();

                while (true)
                {
                    indices.Add(transform.GetSiblingIndex());
                    transform = transform.parent;

                    if (transform == root)
                        break;
                }

                meshFilterPath = indices.ToArray();
            }
        }

        public GameObject GetTargetGameObject(Transform root)
        {
            if (meshFilterPath == null)
                return root.gameObject;

            Transform head = root;
            for (int j = meshFilterPath.Length - 1; j >= 0; j--)
            {
                int index = meshFilterPath[j];
                if (head.childCount <= index)
                    return null;

                head = head.GetChild(index);
            }

            return head.gameObject;
        }

        public MeshFilter ApplyMeshToRenderer(Transform root, Mesh[] meshes)
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                //Finding mesh
                Mesh mesh = meshes[i];
                if(mesh.name == meshName)
                {
                    var target = GetTargetGameObject(root);
                    
                    if(target != null && target.TryGetComponent(out MeshFilter meshFilter))
                    {
                        meshFilter.sharedMesh = mesh;
                        return meshFilter;
                    }
                }
            }
            return null;
        }

        public override int GetHashCode() => meshName.GetHashCode();
#endif
    }
}
