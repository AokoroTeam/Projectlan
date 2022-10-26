using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Aokoro.AI
{
    public abstract class AIAgent : MonoBehaviour
    {
        public IAgentController Controller { get; set; }

        public virtual Vector3 Position { get => transform.position; set => transform.position = value; }
        
        protected virtual void Awake()
        {
            
        }

        public virtual Vector3 MoveAgentTo(Vector3 position)
        {
            transform.position = position;
            return position;
        }

        public abstract void BeforeAwake();
        public abstract void BeforeDestroy();
        public abstract bool IsDespawnabled();

    }
}