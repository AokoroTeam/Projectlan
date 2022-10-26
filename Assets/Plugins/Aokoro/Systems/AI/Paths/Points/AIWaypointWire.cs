using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.AI;
using System.Linq;

namespace Aokoro.AI.Paths
{
    [ExecuteInEditMode]
    public class AIWaypointWire : MonoBehaviour
    {
        [Header("Waypoints")]
        [SerializeField, ReadOnly]
        private AIWaypoint[] spawners;
        [SerializeField, ReadOnly]
        private AIWaypoint[] middles;
        [SerializeField, ReadOnly]
        private AIWaypoint[] despawners;

        private List<AIWaypoint> allWaypoints;

        [Header("Waypoints")]
        [SerializeField]
        private float distancePower = 1;

        public AIWaypoint[] Spawers => spawners;
        public AIWaypoint[] Middles => middles;
        public AIWaypoint[] Despawners => despawners;

        private Dictionary<AIWaypointPair, AIWaypointLink> links;

        public List<AIWaypoint> AllWaypoints
        {
            get
            {
                if (allWaypoints == null)
                    allWaypoints = new List<AIWaypoint>();

                return allWaypoints;
            }
        }
        public Dictionary<AIWaypointPair, AIWaypointLink> Links
        {
            get
            {
                if (links == null)
                    links = new Dictionary<AIWaypointPair, AIWaypointLink>();

                return links;
            }
        }

        public void RegisterWaypoint(AIWaypoint waypoint)
        {
            if(!AllWaypoints.Contains(waypoint))
                AllWaypoints.Add(waypoint);

            BuildWire();
        }

        public void UnregisterWaypoint(AIWaypoint waypoint)
        {
            if (!AllWaypoints.Remove(waypoint))
                AllWaypoints.Add(waypoint);

            BuildWire();
        }

        [Button("Build")]
        public void BuildWire()
        {
            List<AIWaypoint> spawners = new List<AIWaypoint>();
            List<AIWaypoint> middles = new List<AIWaypoint>();
            List<AIWaypoint> despawners = new List<AIWaypoint>();
            
            foreach (var waypoint in AllWaypoints)
            {
                if (waypoint.behavior.HasFlag(WaypointBehavior.Spawn))
                    spawners.Add(waypoint);
                if (waypoint.behavior.HasFlag(WaypointBehavior.Middle))
                    middles.Add(waypoint);
                if (waypoint.behavior.HasFlag(WaypointBehavior.Despawn))
                    despawners.Add(waypoint);
            }

            this.spawners = spawners.ToArray();
            this.despawners = despawners.ToArray();
            this.middles = middles.ToArray();
        }

        public AIWaypoint GetNextWaypointFrom(Vector3 position, WaypointBehavior behavior, int areaMask = NavMesh.AllAreas)
        {
            AIWaypoint[] waypoints = behavior switch
            {
                WaypointBehavior.Despawn => despawners,
                WaypointBehavior.Spawn => spawners,
                _ => middles,
            };

            Dictionary<float, AIWaypoint> probabilities = new Dictionary<float, AIWaypoint>();
            float maxDesirability = 0;

            for (int i = 0; i < waypoints.Length; i++)
            {
                AIWaypoint toward = waypoints[i];

                float desirability = GetProbability((position - toward.Zone.center).sqrMagnitude);

                maxDesirability += desirability;
                probabilities.Add(desirability, toward);
            }

            return ChooseRandomWaypoint(probabilities, maxDesirability);
        }

        public AIWaypoint GetNextWaypointFrom(AIWaypoint from, WaypointBehavior behavior, int areaMask = NavMesh.AllAreas)
        {
            AIWaypoint[] waypoints = behavior switch
            {
                WaypointBehavior.Despawn => despawners,
                WaypointBehavior.Spawn => spawners,
                _ => middles,
            };

            Dictionary<float, AIWaypoint> probabilities = new Dictionary<float, AIWaypoint>();
            float maxDesirability = 0;

            for (int i = 0; i < waypoints.Length; i++)
            {
                AIWaypoint toward = waypoints[i];
                if (toward == from)
                    continue;
                float desirability = GetLinkProbability(from, toward, areaMask);
                maxDesirability += desirability;
                probabilities.Add(desirability, toward);
            }

            return ChooseRandomWaypoint(probabilities, maxDesirability);
        }

        private static AIWaypoint ChooseRandomWaypoint(Dictionary<float, AIWaypoint> probabilities, float maxDesirability)
        {
            float rng = Random.Range(0, maxDesirability);

            foreach (var kv in probabilities)
            {
                float desirability = kv.Key;
                if (rng < desirability)
                    return kv.Value;

                rng -= desirability;
            }

            //Should not get here
            return probabilities.Last().Value;
        }

        public float GetLinkProbability(AIWaypoint a, AIWaypoint b, int areaMask = NavMesh.AllAreas)
        {
            var link = GetLink(a, b, areaMask);
            return GetLinkProbability(link); 

        }
        public float GetLinkProbability(AIWaypointLink link) => GetProbability(link.SqrDistance) * link.A.probability * link.B.probability;
        public float GetProbability(float distance) => Mathf.Pow(1 / distance, distancePower);
        public void ClearLinksCache()
        {
            links.Clear();
        }

        public AIWaypointLink GetLink(AIWaypoint a, AIWaypoint b, int areaMask = NavMesh.AllAreas)
        {
            AIWaypointPair key = new AIWaypointPair(a, b, areaMask);
            if (!Links.TryGetValue(key, out AIWaypointLink link))
            {
                link = new AIWaypointLink(a, b, areaMask);
                Links.Add(key, link);
            }

            return link;
        }
        public struct AIWaypointPair
        {
            public AIWaypoint a;
            public AIWaypoint b;
            public int areaMask;

            public AIWaypointPair(AIWaypoint a, AIWaypoint b, int areaMask = NavMesh.AllAreas)
            {
                this.a = a;
                this.b = b;
                this.areaMask = areaMask;
            }

            public override bool Equals(object obj)
            {
                if (obj is AIWaypointPair pair)
                    return this.areaMask == pair.areaMask && 
                        (this.a == pair.a && this.b == pair.b || this.a == pair.b && this.b == pair.a);

                return false;
            }

            public override int GetHashCode() => base.GetHashCode();
        }
    }

    public struct AIWaypointLink
    {
        public Vector3[] corners;

        public float SqrDistance;
        public float Distance => Mathf.Sqrt(SqrDistance);

        public AIWaypoint A { get; }
        public AIWaypoint B { get; }

        public NavMeshPath path;

        public AIWaypointLink(AIWaypoint a, AIWaypoint b, int areaMask)
        {
            path = new NavMeshPath();
            
            Vector3 aPos = a.Zone.ClosestPoint(b.Zone.center);
            Vector3 bPos = b.Zone.ClosestPoint(a.Zone.center);

            bool sampled = NavMesh.SamplePosition(aPos, out NavMeshHit lastHit, 5, areaMask);
            sampled &= NavMesh.SamplePosition(bPos, out NavMeshHit currentHit, 5, areaMask);

            if (sampled && NavMesh.CalculatePath(lastHit.position, currentHit.position, areaMask, path))
            {
                corners = path.corners;
                int pathSize = corners.Length;

                SqrDistance = 0;
                for (int k = 1; k < pathSize; k++)
                    SqrDistance += (corners[k - 1] - corners[k]).sqrMagnitude;

            }
            else
            {
                corners = new Vector3[] { aPos, bPos };
                SqrDistance = (aPos - bPos).sqrMagnitude;
            }
            A = a;
            B = b;
        }
    }
}
