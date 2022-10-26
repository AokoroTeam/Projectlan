using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Pooling
{
    public enum PoolResourceType
    {
        Reference,
        ResourcePath
    }
    [System.Serializable]
    public class PoolResource
    {
        public GameObject Resource
        {
            get
            {
                if (_resource == null)
                {
                    if(loadResource == null)
                    {
                        switch(resourceType)
                        {
                            case PoolResourceType.Reference:
                                SetResourceWithRef(sResource);
                                break;
                            case PoolResourceType.ResourcePath:
                                SetResourceWithPath(sPath);
                                break;
                        }
                    }
                    _resource = loadResource.Invoke();
                }

                return _resource;
            }
        }

        private GameObject _resource;
        private Func<GameObject> loadResource;
        
        public bool IsNull => Resource == null;

        public PoolResourceType resourceType;

        [SerializeField]
        string sPath;
        [SerializeField]
        GameObject sResource;

        public PoolResource(GameObject model)
        {
            SetResourceWithRef(model);
        }

        public PoolResource(string resourcePath)
        {
            SetResourceWithPath(resourcePath);
        }

        private void SetResourceWithRef(GameObject model)
        {
            sResource = model;
            resourceType = PoolResourceType.Reference;
            if (model == null)
                Debug.LogError("[Pooling System] model is null");

            loadResource = () => model;
        }

        private void SetResourceWithPath(string resourcePath)
        {
            sPath = resourcePath;
            resourceType = PoolResourceType.ResourcePath;

            loadResource = () =>
            {
                GameObject resource = Resources.Load<GameObject>(resourcePath);
                if (resource == null) PoolingManager.LogError($"There is no prefab at path{resourcePath}");
                return resource;
            };
        }

        public PoolResource() { }

        public static implicit operator GameObject(PoolResource poolModel) => poolModel.Resource;
        public static implicit operator PoolResource(GameObject poolModel) => new PoolResource(poolModel);
        public static implicit operator PoolResource(string poolPath) => new PoolResource(poolPath);
    }
}