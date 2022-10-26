using Aokoro;
using Newtonsoft.Json.Linq;
using Realit.Library;
using Realit.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Builder
{
    [AddComponentMenu("Realit/Builder/Build/Assets data builder")]
    public class AssetsDataBuilder : Singleton<AssetsDataBuilder>, IDataBuilder
    {
        public DataSection Section => DataSection.Assets;

        public bool IsValid => true;

        public JObject Serialize()
        {
            Dictionary<string, List<RealitAssetData>> assetsData = new();

            var assets = FindObjectsOfType<RealitAsset>();
            
            //Enumerating assets
            foreach (var asset in assets)
            {
                var assetData = asset.ExtractData();

                if(assetsData.ContainsKey(assetData.address))
                    assetsData[assetData.address].Add(assetData);
                else
                    assetsData[assetData.address] = new List<RealitAssetData>() { assetData };

            }
            JObject json = new();
            foreach (var reference in assetsData)
            {
                List<RealitAssetData> instancesData = reference.Value;

                JToken[] dataObjects = new JToken[instancesData.Count];
                for (int i = 0; i < reference.Value.Count; i++)
                {
                    dataObjects[i] = instancesData[i].ToJson();
                }
                json.Add(new JProperty(reference.Key, dataObjects));
            }
            return json;
        }

        protected override void OnExistingInstanceFound(AssetsDataBuilder existingInstance)
        {

        }

    }
}
