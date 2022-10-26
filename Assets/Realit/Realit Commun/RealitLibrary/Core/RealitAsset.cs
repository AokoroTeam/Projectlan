using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Aokoro.ModelExports.Runtime;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Realit.Library
{

    [AddComponentMenu("Realit/Library/RealitPrefab"), ExecuteInEditMode]
    public class RealitAsset : MonoBehaviour
    {
#if UNITY_EDITOR
        [ReadOnly]
        public string projectPath;
#endif

        [SerializeField, ReadOnly]
        public string addressableKey;

        public RealitAsset OnAssetCreated(string addressableKey)
        {
            this.addressableKey = addressableKey;

            if (TryGetComponent(out ModelExportComponent modelExporter))
            {
                if (Application.isPlaying)
                    DestroyImmediate(modelExporter);
                else
                    Destroy(modelExporter);
            }

            return this;
        }


        private void OnDestroy()
        {
            if (Application.isPlaying)
                RealitLibraryManager.ReleaseAsset(this);
        }


#if IS_BUILDER
        public virtual RealitAssetData ExtractData()
        {
            RealitAssetData asset = new RealitAssetData(addressableKey)
                .AddVector3Property("position", transform.position)
                .AddVector3Property("rotation", transform.rotation.eulerAngles)
                .AddVector3Property("scale", transform.localScale);

            return asset;
        }
#endif

#if IS_READER
        public virtual void ApplyData(JContainer json)
        {
            RealitAssetData realitAssetData = new RealitAssetData(addressableKey);
            realitAssetData.FromJson(json);

            transform.position = realitAssetData.GetProperty<Vector3Property>("position").Value;
            transform.eulerAngles = realitAssetData.GetProperty<Vector3Property>("rotation").Value;
            transform.localScale = realitAssetData.GetProperty<Vector3Property>("scale").Value;
        }
#endif
    }

}