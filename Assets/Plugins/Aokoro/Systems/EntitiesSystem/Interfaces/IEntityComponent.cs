using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Entities
{

    public interface IEntityComponent 
    {
        string ComponentName { get; }
    }

    public interface IUpdateEntityComponent : IEntityComponent
    {
        void OnUpdate();
    }
    public interface ILateUpdateEntityComponent : IEntityComponent
    {
        void OnLateUpdate();
    }
    public interface IFixedUpdateEntityComponent : IEntityComponent
    {
        void OnFixedUpdate();
    }
}
