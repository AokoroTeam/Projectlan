using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Aokoro.AI.Paths
{
    [System.Serializable]
    public class AIAgentPath
    {

        public AIWaypoint Destination
        {
            get => _destination;
            private set
            {
                Vector3 pos = value.GetPositionInsideZone();
                NavMesh.SamplePosition(pos, out NavMeshHit hit, 10, areaMask);
                DestinationPosition = hit.position;
                _destination = value;
            }
        }

        private AIWaypoint _destination;

        public Vector3 DestinationPosition;

        private readonly AIWaypointWire wire;
        public int CurrentPathStep { get; private set; }
        public int PathSize { get; private set; }

        private readonly int areaMask;

        public AIAgentPath(AIAgent agent, AIWaypointWire wire, int pathSize, int areaMask)
        {
            this.wire = wire;
            this.PathSize = pathSize;
            this.areaMask = areaMask;

            this.CurrentPathStep = 0;
            this.Destination = wire.GetNextWaypointFrom(agent.Position, WaypointBehavior.Middle, areaMask);
        }

        public bool MoveNext()
        {
            CurrentPathStep++;

            if (CurrentPathStep < PathSize)
            {
                Destination = wire.GetNextWaypointFrom(Destination, WaypointBehavior.Middle, areaMask);
                return true;
            }
            else if (CurrentPathStep == PathSize)
            {
                Destination = wire.GetNextWaypointFrom(Destination, WaypointBehavior.Despawn, areaMask);
                return true;
            }

            return false;
        }

    }
}
