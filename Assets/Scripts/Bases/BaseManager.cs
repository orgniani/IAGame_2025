using Managers;
using Miners;
using System.Collections.Generic;
using UnityEngine;

namespace Bases
{
    public class BaseManager : MonoBehaviour
    {
        [SerializeField] private PathfindingManager pathfindingManager;
        [SerializeField] private List<BasePoint> basePoints = new();

        private void Awake()
        {
            foreach (var point in basePoints)
            {
                if (point != null)
                    RegisterBasePoint(point);
            }
        }

        public void RegisterBasePoint(BasePoint point)
        {
            if (!basePoints.Contains(point))
                basePoints.Add(point);

            point.SetManager(this);
        }

        public void UnregisterBasePoint(BasePoint point)
        {
            if (basePoints.Contains(point))
                basePoints.Remove(point);
        }

        public BasePoint GetBestAvailableBase(Vector3 minerPosition, Miner requester)
        {
            BasePoint bestPoint = null;
            int bestPathLength = int.MaxValue;

            foreach (var point in basePoints)
            {
                if (point == null || point.IsOccupied || point.ReservedBy == requester)
                    continue;

                var path = pathfindingManager.CreatePath(minerPosition, point.Position);
                if (path == null)
                    continue;

                int pathLength = path.Count;
                if (pathLength < bestPathLength)
                {
                    bestPoint = point;
                    bestPathLength = pathLength;
                }
            }

            if (bestPoint != null && bestPoint.TryReserve(requester))
                return bestPoint;

            return null;
        }
    }
}