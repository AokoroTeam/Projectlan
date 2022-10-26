using Newtonsoft.Json.Linq;
using Realit.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Models;
using NaughtyAttributes;
using Realit.Library;
using System;

namespace Realit.Reader.Managers
{
    [AddComponentMenu("Realit/Reader/Data/Asset data reader")]
    public class AssetsDataReader : MonoBehaviour, IDataReader
    {
        public DataSection Section => DataSection.Assets;
        [ShowNativeProperty]
        public bool Done => currentLoadings == 0;
        [ShowNativeProperty]
        public bool Skipped { get; set; }
        [ShowNonSerializedField]
        private int currentLoadings;


        private Transform assetsParent;

        private void Awake()
        {
            currentLoadings = -1;
            if(assetsParent == null)
            {
                assetsParent = new GameObject("Assets").transform;
                assetsParent.position = Vector3.zero;
            }
        }


        public void ApplyData(JToken data)
        {
            currentLoadings = 0;
            foreach (JProperty assetDescription in data)
            {
                string assetPath = assetDescription.Name;
                var instances = assetDescription.Values();

                foreach (JToken instance in instances)
                {
                    currentLoadings++;
                    RealitLibraryManager.LoadAsset(assetPath, ctx => { OnLoaded(ctx, instance as JObject); }, OnError);
                }
            }

            Resources.UnloadUnusedAssets();
        }


        private void OnLoaded(RealitAsset asset, JObject json)
        {
            asset.ApplyData(json);
            asset.transform.SetParent(assetsParent);

            currentLoadings--;
        }

        private void OnError()
        {
            currentLoadings--;
        }
    }
}