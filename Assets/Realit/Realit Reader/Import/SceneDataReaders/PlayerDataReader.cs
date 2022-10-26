using Newtonsoft.Json.Linq;
using Realit.Models;
using Realit.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Realit.Reader.Managers
{
    [AddComponentMenu("Realit/Reader/Data/Player data reader")]
    public class PlayerDataReader : MonoBehaviour, IDataReader
    {
        [SerializeField]
        private AssetReferenceGameObject player;   
        public DataSection Section => DataSection.Player;

        public bool Done => IsPlayerInstantiated;

        public bool Skipped { get; set; }

        private bool IsPlayerInstantiated = false;

        public void ApplyData(JToken data)
        {
            IsPlayerInstantiated = false;
            Debug.Log("[Player] Loading player...");
            

            var pos = new Vector3().Deserialize(data["pos"]);
            var rot = Quaternion.LookRotation(new Vector3().Deserialize(data["rot"]), Vector3.up);

            var asyncOperation = player.InstantiateAsync(pos, rot);
            asyncOperation.Completed += p =>
            {
                IsPlayerInstantiated = true;
                RealitReader.OnPlayerInstantiated(p.Result);
                Debug.Log("[Player] Player loaded");
            };
        }
    }
}