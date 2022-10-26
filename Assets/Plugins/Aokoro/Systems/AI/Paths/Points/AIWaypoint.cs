using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Aokoro.Sequencing;
using Random = UnityEngine.Random;

namespace Aokoro.AI.Paths
{
    [Flags]
    public enum WaypointBehavior
    {
        Spawn = 1,
        Despawn = 2,
        Middle = 4,
    }
    [AddComponentMenu("Aokoro/AI/Paths/Waypoint"), ExecuteInEditMode]
    public class AIWaypoint : MonoBehaviour
    {
        [SerializeField]
        private AIWaypointWire container;

        public Action<AIAgent> OnAgentEnter;
        public Action<AIAgent> OnAgentOut;

        
        public WaypointBehavior behavior = WaypointBehavior.Middle;

        [SerializeField] private LayerMask layer;
        [SerializeField] public int probability;
        [SerializeField] private AIWaypointActivity activity;

        public Bounds Zone { get => GetWorldZone(); }
        [SerializeField] public Bounds _zone;

        private Dictionary<AIAgent, ISequencer> agentsActivity;

        private void Reset()
        {
            Debug.Log("reset");
            probability = 100;
            behavior = WaypointBehavior.Middle;
            _zone = new Bounds(Vector3.zero, Vector3.one);
        }
        private void Awake()
        {
            container = GetComponentInParent<AIWaypointWire>();
            if (container == null)
            {
                if (Application.isPlaying)
                    Destroy(this);
                else
                    DestroyImmediate(this);
            }
            else
                container.RegisterWaypoint(this);
        }

        private void OnDestroy()
        {
            container?.UnregisterWaypoint(this);
        }
        public bool UpdateAgentWaypoint(AIAgent aIAgent)
        {
            if (agentsActivity.ContainsKey(aIAgent))
                return !agentsActivity[aIAgent].IsRunning();
            else
                return true;
        }

        private Vector3 GetZoneCenter()
        {
            const int MaxRayLenght = 15;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, MaxRayLenght, layer))
                return hit.point;
            else
                return transform.position;
        }

        public Bounds GetWorldZone() => new Bounds(GetZoneCenter(), _zone.size);

        public virtual Vector3 GetPositionInsideZone()
        {
            var rect = GetWorldZone();
            float x = Random.Range(rect.min.x, rect.max.x);
            float z = Random.Range(rect.min.z, rect.max.z);
            Vector3 worldPos = new Vector3(x, GetZoneCenter().y, z);
            
            return worldPos;
        }

        public void AgentIn(AIAgent aIAgent)
        {
            if (activity != null)
            {
                var sequence = activity.ActivitySequence(aIAgent);
                agentsActivity.Add(aIAgent, sequence.Play(aIAgent, activity, this));
            }

            OnAgentEnter?.Invoke(aIAgent);
        }

        public void AgentOut(AIAgent aIAgent)
        {
            ISequencer sequencer = agentsActivity[aIAgent];
            if (sequencer.IsRunning() && agentsActivity.Remove(aIAgent))
                sequencer.Stop();

            OnAgentOut?.Invoke(aIAgent);
        }
    }
}
