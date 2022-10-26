using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Entities
{
    public interface IUpdateEntityComponent<T> : IEntityComponent<T>, IUpdateEntityComponent where T : Entity
    { }
    public interface ILateUpdateEntityComponent<T> : IEntityComponent<T>, ILateUpdateEntityComponent where T : Entity
    { }
    public interface IFixedUpdateEntityComponent<T> : IEntityComponent<T>, IFixedUpdateEntityComponent where T : Entity
    { }

    public interface IEntityComponent<T> : IEntityComponent where T : Entity
    {

        T Manager { get; set; }
        void Initiate(T manager);
    }
}
