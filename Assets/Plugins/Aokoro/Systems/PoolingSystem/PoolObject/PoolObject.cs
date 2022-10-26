using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Aokoro.Pooling
{
    public class PoolObject : MonoBehaviour
    {
        [ShowNativeProperty]
        private Pool _pool => Pool;

        public Pool Pool { get; internal set; }

        private IPoolObjectCallbackReceiver[] receivers;


        [ShowNativeProperty]
        public virtual bool ActiveInPool
        {
            get => activeInPool; 
            internal set
            {
                gameObject.SetActive(value);
                activeInPool = value;
            }
        }
        private bool activeInPool;

        public bool BeingDestroyed { get; private set; }

        private void Awake()
        {
            SyncCallbackReceivers();
        }

        private void OnDestroy()
        {
            BeingDestroyed = true;
            if (!PoolingManager.Quitting && Pool != null)
                PoolingManager.LogError("This pooled object has been manually destroyed. You should never destroy a Pooled object");
        }
        public void SyncCallbackReceivers()
        {
            receivers = GetComponents<IPoolObjectCallbackReceiver>();
            for (int i = 0; i < receivers.Length; i++)
                receivers[i].poolObject = this;
        }

        public void PODestroy()
        {
            Pool.PODestroy(this);
            for (int i = 0; i < receivers.Length; i++)
                receivers[i].OnPoolDestroy();

        }

        #region Pool management
        internal void OnPOAwake()
        {
            ActiveInPool = true;
        }

        internal void OnPODestroy()
        {
            ActiveInPool = false;
            transform.localPosition = Vector3.zero;

        }

        internal void OnPOClean()
        {
            Pool = null;
            Destroy(gameObject);
        }

        #endregion

        

        public static implicit operator GameObject(PoolObject poolObject) => poolObject.gameObject;
    }
}
