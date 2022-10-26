using Aokoro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Realit.Library
{
    public class RealitLibraryManager : Singleton<RealitLibraryManager>
    {
        private Dictionary<RealitAsset, AsyncOperationHandle> handles = new();

        public static void LoadAsset(string addressablePath, Action<RealitAsset> OnLoaded, Action OnError)
        {
            
            var op = Addressables.LoadAssetAsync<GameObject>(addressablePath);
            op.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    RealitAsset realitAsset = GameObject.Instantiate(handle.Result).GetComponent<RealitAsset>();
                    Instance.handles.Add(realitAsset.OnAssetCreated(addressablePath), handle);

                    OnLoaded(realitAsset);
                    
                }
                else
                {
                    OnError?.Invoke();
                    Addressables.Release(handle);
                    RealitLibrary.LogError(handle.OperationException.Message);
                }
            };
        }

        public static void ReleaseAsset(RealitAsset asset)
        {
            if(Instance.handles.Remove(asset, out var asyncOperationHandle))
                Addressables.Release(asyncOperationHandle);
        }
        protected override void OnExistingInstanceFound(RealitLibraryManager existingInstance)
        {
            Destroy(gameObject);
        }
    }
}
