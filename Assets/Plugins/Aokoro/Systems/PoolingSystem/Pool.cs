using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aokoro.Pooling;
using System.Linq;
using NaughtyAttributes;

namespace Aokoro.Pooling
{
    public enum PoolEmptyBehavior
    {
        DontInstantiate,
        Loop,
        CreateTemporary,
        Extend
    }

    [AddComponentMenu("Aokoro/PoolingSystem/Pool"), DefaultExecutionOrder(-50)]
    public class Pool : MonoBehaviour
    {
        [SerializeField]
        private PoolResource resource;
        [SerializeField]
        private int m_capacity;
        [SerializeField]
        private bool m_loadOnAwake;

        //La list
        private HashSet<PoolObject> _posReserve;
        private HashSet<PoolObject> _posInstantiated;
        private HashSet<PoolObject> POsReserve
        {
            get
            {
                if (_posReserve == null)
                    _posReserve = new HashSet<PoolObject>();

                return _posReserve;
            }
        }
        private HashSet<PoolObject> POsInstantiated
        { 
            get
            {
                if (_posInstantiated == null)
                    _posInstantiated = new HashSet<PoolObject>();

                return _posInstantiated;
            }
        }
        public int Capacity { get => m_capacity; internal set => m_capacity = value; }
        public bool LoadOnAwake { get => m_loadOnAwake; internal set => m_loadOnAwake = value; }

        public int ObjectCount => POsReserve.Count + POsInstantiated.Count;


        public PoolEmptyBehavior EmptyBehavior = PoolEmptyBehavior.DontInstantiate;

        private bool generateOnAwake = false;

        private Transform container;

        public static Pool Create(GameObject poolParent, int capacity, PoolResource resource)
        {
            GameObject poolGO = new GameObject($"[Pool] {resource.Resource.name}");
            poolGO.transform.SetParent(poolParent.transform);
            Pool pool = poolGO.AddComponent<Pool>();

            pool.Initialize(capacity, resource, pool.transform);
            return pool;
        }

        private void OnEnable()
        {
            PoolingManager.Instance.Check += Check;
        }

        private void OnDisable()
        {
            if(!PoolingManager.Quitting)
                PoolingManager.Instance.Check -= Check;   
        }

        private void Check(PoolingManager manager)
        {
            foreach (var po in POsReserve)
            {
                if (po.gameObject.activeInHierarchy && !po.ActiveInPool)
                {
                    PoolingManager.LogError(
                        "This pooled object has been activated outside of its Pool. Disabling it",
                        po.gameObject);
                    po.gameObject.SetActive(false);
                }
            }
            foreach (var po in POsInstantiated)
            {
                if (!po.gameObject.activeInHierarchy && po.ActiveInPool)
                {
                    PoolingManager.LogError(
                        "This pooled object has been deactivated outside of its Pool. Activating it",
                        po.gameObject); 
                    po.gameObject.SetActive(true);
                }
            }
        }

        private void Awake()
        {
            if (generateOnAwake)
                Initialize(m_capacity, resource, null);
        }
        private void Initialize(int capacity, PoolResource resource, Transform container)
        {
            if(container == null)
            {
                container = new GameObject($"[Pool] {resource.Resource.name}").transform;
                container.SetParent(transform);
            }

            this.container = container;
            if (resource.IsNull)
            {
                Debug.LogError($"[Pool System] Model is null. Error from {transform.name}", gameObject);
                return;
            }

            this.m_capacity = capacity;
            this.resource = resource;

            for (int i = 0; i < capacity; i++)
                CreateRessource();
        }

        //Ajoute un object dans la pool
        private PoolObject CreateRessource()
        {
            if (resource.IsNull)
            {
                PoolingManager.LogError("Model is null");
                return null;
            }

            GameObject instance = GameObject.Instantiate(resource.Resource, container);
            instance.name = $"[PoolObject] {resource.Resource.name}_{ObjectCount + 1}";
            instance.transform.localPosition = Vector3.zero;

            //Rajoute un script poolObject si ce n'est pas deja fait
            if (!instance.TryGetComponent(out PoolObject po))
                po = instance.AddComponent<PoolObject>();

            po.Pool = this;
            po.OnPODestroy();
            po.ActiveInPool = false;

            AddPOToReserve(po);
            return po;
        }

        private void DeleteRessource(PoolObject po)
        {
            if (po.Pool != this)
                return;

            POsInstantiated.Remove(po);
            POsReserve.Remove(po);

            po.OnPOClean();
        }
        
        //Sort un object de la Pool
        public PoolObject POInstantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            PoolObject po = GetNextPO();
            if (po == null)
                return null;

            po.transform.SetParent(parent);
            po.transform.SetPositionAndRotation(position, rotation);
            
            RemovePOFromReserve(po);

            po.OnPOAwake();
            return po;
        }

        public PoolObject POInstantiate() => POInstantiate(Vector3.zero, Quaternion.identity, null);
        public PoolObject POInstantiate(Transform parent) => POInstantiate(Vector3.zero, Quaternion.identity, parent);
        public PoolObject POInstantiate(Vector3 position, Quaternion rotation) => POInstantiate(position, rotation, null);


        public void PODestroy(PoolObject poolObject)
        {
            poolObject.transform.SetParent(container);
            AddPOToReserve(poolObject);
            poolObject.OnPODestroy();
        }

        private void AddPOToReserve(PoolObject poolObject)
        {
            POsInstantiated.Remove(poolObject);
            POsReserve.Add(poolObject);
        }

        private void RemovePOFromReserve(PoolObject poolObject)
        {
            POsInstantiated.Add(poolObject);
            POsReserve.Remove(poolObject);
        }
        
        private PoolObject GetNextPO()
        {
            //No avaiable PoolObject
            if (POsReserve.Count == 0)
            {
                switch(EmptyBehavior)
                {
                    case PoolEmptyBehavior.DontInstantiate:
                        return null;
                    case PoolEmptyBehavior.Extend:
                        Capacity++;
                        return CreateRessource();
                    case PoolEmptyBehavior.CreateTemporary:
                        return CreateRessource();
                    case PoolEmptyBehavior.Loop:
                        PoolObject last = POsInstantiated.First();
                        PODestroy(last);
                        return GetNextPO();
                }
            }

            return POsReserve.First();
        }

        public void UpdateCapacity(int newCapacity)
        {
            if(newCapacity <= 0)
            {
                PoolingManager.LogError("Capacity can only be superior or equal to 1");
                return;
            }
            if (newCapacity == Capacity)
                return;

            int delta = newCapacity - Capacity;
            int absDelta = Mathf.Abs(delta);

            for (int i = 0; i < absDelta; i++)
            {
                if (delta < 0)
                    DeleteRessource(POsInstantiated.Count > 0 ? POsInstantiated.First() : POsReserve.Last());
                else
                    CreateRessource();
            }

            Capacity = newCapacity;
        }
        //Pour vider une pool
        public void Clear()
        {
            foreach (PoolObject po in POsReserve)
                    po?.OnPOClean();
            
            POsReserve.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }

}
