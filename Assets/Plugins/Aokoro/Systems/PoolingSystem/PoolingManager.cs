using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Aokoro.Pooling
{
    [AddComponentMenu("Aokoro/PoolingSystem/Pooling Manager")]
    public class PoolingManager : Singleton<PoolingManager>
    {
        private Dictionary<PoolResource, Pool> pools = new Dictionary<PoolResource, Pool>();
        public event Action<PoolingManager> Check;


#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            UnityEditor.EditorApplication.playModeStateChanged += ModeChanged;
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            foreach(var pool in pools)
                pool.Value.Clear();

            pools.Clear();
        }

        private void Update()
        {
            Check?.Invoke(this);
        }

        public Pool CreatePool(string path, int size) => CreatePool(new PoolResource(path), size);
        public Pool CreatePool(GameObject model, int size) => CreatePool(new PoolResource(model), size);
        //Pour des pools comumnes
        public Pool CreatePool(PoolResource resource,int size)
        {
            if(HasPool(resource.Resource,out Pool existingPool))
            {
                if(existingPool.Capacity < size)
                {
                    existingPool.Clear();
                    existingPool = Pool.Create(gameObject, size, resource);
                }
                return existingPool;
            }

            Pool pool = Pool.Create(gameObject, size, resource);
            pools.Add(resource, pool);

            return pool;
        }
        
        //Juste pour savoir si une pool existe
        public bool HasPool(GameObject model, out Pool pool)
        {
            pool = new Pool();

            var lambda = new System.Func<KeyValuePair<PoolResource, Pool>,bool> (ctx => ctx.Key.Resource == model);
            bool hasPool = pools.Any(lambda);

            if (hasPool)
                pool = pools.First(lambda).Value;

            return hasPool;
        }
        public bool HasPool(PoolResource resource, out Pool pool) => pools.TryGetValue(resource, out pool);

        public PoolObject POInstantiate(GameObject instance, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (pools.TryGetValue(instance, out Pool pool))                
                return pool.POInstantiate(position, rotation, parent);
            else
            {
                LogError("No pool found for object " + instance.name);
                return null;
            }
        }
        public PoolObject POInstantiate(GameObject instance) => POInstantiate(instance, instance.transform.position, instance.transform.rotation, null);
        public PoolObject POInstantiate(GameObject instance, Transform parent) => POInstantiate(instance, instance.transform.position, instance.transform.rotation, parent);
        public PoolObject POInstantiate(GameObject instance, Vector3 position, Quaternion rotation) => POInstantiate(instance, position, rotation, null);

        public static bool Quitting;
        

#if UNITY_EDITOR
        private static void ModeChanged(UnityEditor.PlayModeStateChange msg)
        {
            if (msg == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                UnityEditor.EditorApplication.playModeStateChanged -= ModeChanged;
                Quitting = true;
            }
        }
#endif

        protected void OnApplicationQuit()
        {
            Quitting = true;
        }

        protected override void OnExistingInstanceFound(PoolingManager existingInstance)
        {
            Destroy(this);
        }

        internal static void LogError(string message, UnityEngine.Object context = null) => Debug.LogError($"[Pooling system] {message}", context);
        internal static void LogWarning(string message, UnityEngine.Object context = null) => Debug.LogWarning($"[Pooling system] {message}", context);
    }
}
