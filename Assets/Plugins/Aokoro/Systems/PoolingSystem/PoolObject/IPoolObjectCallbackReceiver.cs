using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aokoro.Pooling
{
    public interface IPoolObjectCallbackReceiver
    {
        PoolObject poolObject { get; set; }
        void OnPoolAwake();
        void OnPoolDestroy();
    }
}