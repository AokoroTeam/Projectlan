using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using Aokoro.AI.Grid;
using Aokoro.AI.Paths;

namespace Aokoro.AI
{
    public enum AgentControllerState
    {
        WellPopulated,
        UnderPopulated,
        OverPopulated
    }

    public abstract class AIAgentController<T, U> : MonoBehaviour, IAgentController
        where T : AIAgent
        where U : AIGrid
    {
        [SerializeField]
        protected U grid;
        [BoxGroup("Agents")]
        [SerializeField, Tooltip("Agent/unity")]
        private float desiredDensity;
        [BoxGroup("Agents")]
        [SerializeField, Range(0,100)]
        private int densityMargin;
        [BoxGroup("Agents")]
        [SerializeField, Min(0)]
        protected int MaxAgent;
        
        [ShowNativeProperty, SerializeField]
        protected virtual int DesiredAgentCount => Mathf.Min(MaxAgent, Mathf.CeilToInt(desiredDensity * grid.Area));

        public virtual float MinDensity => desiredDensity - (densityMargin * 0.01f * desiredDensity);
        public virtual float MaxDensity => desiredDensity + (densityMargin * 0.01f * desiredDensity);

        public float Density
        {
            get
            {
                if (Application.isPlaying && HasDensityChanged())
                    density = GetDensity();

                return density;
            }
        }

        [ShowNonSerializedField]
        protected float density;
        protected float surfaceSize;

        protected AgentControllerState state;

        IEnumerable<AIAgent> IAgentController.Agents => activeAgents;

        private List<T> activeAgents;
        protected int ActiveAgentCount => activeAgents.Count;


        protected virtual void Awake()
        {
            activeAgents = new List<T>();
            grid = GetComponent<U>();
            grid.BuildData();

            HandleAgentsQuantity(DesiredAgentCount);
        }

        public virtual void Update()
        {
            grid.UpdateGrid();
            
            state = EvaluateState();
            int delta = EvaluateAgentDelta(state);

            HandleAgentsQuantity(delta);
            UpdateAgents();
        }

        #region Density

        private void HandleAgentsQuantity(int quantityDelta)
        {
            int absoluteDelta = Mathf.Abs(quantityDelta);

            for (int i = 0; i < absoluteDelta; i++)
            {
                if (quantityDelta > 0)
                    AddAgent();
                else
                    RemoveDestroyableAgent();
            }
        }

        protected bool HasDensityChanged() => true;
        protected float GetDensity() => ActiveAgentCount / grid.Area;

        protected virtual AgentControllerState EvaluateState()
        {
            if (Density > MaxDensity)
                return AgentControllerState.OverPopulated;
            if (Density < MinDensity)
                return AgentControllerState.UnderPopulated;

            return AgentControllerState.WellPopulated;
        }
        protected int EvaluateAgentDelta(AgentControllerState state)
        {
            return state switch
            {
                AgentControllerState.OverPopulated => ActiveAgentCount + DesiredAgentCount,
                AgentControllerState.UnderPopulated => -(ActiveAgentCount - DesiredAgentCount),
                _ => 0,
            };
        }

        #region Agents Creation

        protected virtual void AddAgent()
        {
            T agent = InstantiateNewAgent();
            agent.Controller = this;
            activeAgents.Add(agent);
            agent.BeforeAwake();

            SetupAgent(agent);
        }
        protected abstract T InstantiateNewAgent();
        protected abstract void SetupAgent(T agent);
        #endregion

        #region Agent Destruction

        protected virtual void RemoveDestroyableAgent()
        {
            T agent = GetDestroyableAgent();
            RemoveAgent(agent);
        }

        protected virtual void RemoveAgent(T agent)
        {
            activeAgents.Remove(agent);

            agent.BeforeDestroy();

            DestroyAgent(agent);
        }

        #endregion
        protected virtual T GetDestroyableAgent() 
        {
            var agent = activeAgents.FirstOrDefault(a => a.IsDespawnabled());
            return agent ?? GetMostLikelyDestroyableAgent();
        }

        protected virtual T GetMostLikelyDestroyableAgent() => activeAgents.Last();
        protected abstract void DestroyAgent(T agent);

        #endregion

        private void UpdateAgents()
        {
            int activeAgentCount = ActiveAgentCount;
            T[] agents = activeAgents.ToArray();

            for (int i = 0; i < activeAgentCount; i++)
            {
                try
                {
                    UpdateAgent(agents[i]);
                }
                catch(System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        protected abstract void UpdateAgent(T agent);
    }
}
