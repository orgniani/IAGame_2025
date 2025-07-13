using System.Collections.Generic;
using UnityEngine;
using Nodes;
using System;

namespace Managers
{
    public class PathfindingManager : MonoBehaviourSingleton<PathfindingManager>
    {
        public enum PathfindingStrategy
        {
            BreadthFirst,
            DepthFirst,
            Dijkstra,
            AStar
        }

        [SerializeField] private PathGenerator pathGenerator = new PathGenerator();
        [SerializeField] private PathfindingStrategy pathfindingStrategy = PathfindingStrategy.BreadthFirst;

        private PathNode _destinationNodeCache;

        private List<PathNode> pathNodes;
        private List<PathNode> openNodes;
        private List<PathNode> closedNodes;

        public event Action OnStrategyChanged;

        void Awake ()
        {
            if (pathNodes == null)
                GeneratePath();
            
            openNodes = new List<PathNode>();
            closedNodes = new List<PathNode>();
        }

        void OnDrawGizmos ()
        {
            if (pathNodes == null)
                return;

            Gizmos.color = Color.blue;

            foreach (PathNode pathNode in pathNodes)
            {
                foreach (PathNode adjacentNode in pathNode.AdjacentNodes)
                    Gizmos.DrawLine(pathNode.Position, adjacentNode.Position);
            }           
        }

        
        [ContextMenu("Generate Path")]
        private void GeneratePath ()
        {
            pathNodes = pathGenerator.GenerateNodes();
        }

        public void SetStrategy(PathfindingStrategy newStrategy)
        {
            pathfindingStrategy = newStrategy;
            OnStrategyChanged?.Invoke();

            Debug.Log($"[Pathfinding] Strategy set to {newStrategy}");
        }

        private PathNode FindClosestNode (Vector3 position)
        {
            PathNode closestNode = null;

            float closestSqrDistance = float.MaxValue;

            foreach (PathNode pathNode in pathNodes)
            {
                float sqrDistance = (pathNode.Position - position).sqrMagnitude;

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestNode = pathNode;
                }
            }

            return closestNode;
        }

        private PathNode GetNextOpenNode (PathNode destinationNode)
        {
            if (openNodes.Count == 0)
                return null;

            PathNode openNode = null;

            switch (pathfindingStrategy)
            {
                case PathfindingStrategy.BreadthFirst:
                    openNode = GetNextOpenNodeBreadthFirst();
                    break;
                
                case PathfindingStrategy.DepthFirst:
                    openNode = GetNextOpenNodeDepthFirst();
                    break;

                case PathfindingStrategy.Dijkstra:
                    openNode = GetNextOpenNodeDijkstra();
                    break;
                
                case PathfindingStrategy.AStar:
                    openNode = GetNextOpenNodeAStar(destinationNode);
                    break;
            }

            return openNode;
        }

        private void OpenNode (PathNode node)
        {
            if (openNodes.Contains(node))
                return;

            node.CurrentState = PathNode.State.Open;
            openNodes.Add(node);
        }

        private void CloseNode (PathNode node)
        {
            if (!openNodes.Contains(node))
                return;

            node.CurrentState = PathNode.State.Closed;
            openNodes.Remove(node);
            closedNodes.Add(node);
        }

        private void OpenAdjacentNodes(PathNode parentNode)
        {
            foreach (PathNode pathNode in parentNode.AdjacentNodes)
            {
                if (pathNode.CurrentState != PathNode.State.Unreviewed)
                    continue;

                pathNode.Parent = parentNode;

                switch (pathfindingStrategy)
                {
                    case PathfindingStrategy.Dijkstra:
                    case PathfindingStrategy.AStar:
                        float sqrDistance = (parentNode.Position - pathNode.Position).sqrMagnitude;
                        pathNode.AccumulatedCost = parentNode.AccumulatedCost + sqrDistance * pathNode.CostMultiplier;

                        if (pathfindingStrategy == PathfindingStrategy.AStar)
                        {
                            float h = (_destinationNodeCache.Position - pathNode.Position).sqrMagnitude;
                            pathNode.HeuristicCost = h;
                        }
                        break;
                }

                OpenNode(pathNode);
            }
        }

        private void ResetNodes ()
        {
            foreach (PathNode pathNode in pathNodes)
            {
                if (pathNode.CurrentState == PathNode.State.Unreviewed)
                    continue;

                pathNode.CurrentState = PathNode.State.Unreviewed;
                pathNode.Parent = null;
                pathNode.AccumulatedCost = 0f;
            }

            openNodes.Clear();
            closedNodes.Clear();
        }

        private PathNode GetNextOpenNodeBreadthFirst () => openNodes[0];
        
        private PathNode GetNextOpenNodeDepthFirst () => openNodes[^1];

        private PathNode GetNextOpenNodeDijkstra ()
        {
            PathNode openNode = openNodes[0];

            foreach (PathNode pathNode in openNodes)
            {
                if (pathNode.AccumulatedCost < openNode.AccumulatedCost)
                    openNode = pathNode;
            }
            
            return openNode;
        }

        private PathNode GetNextOpenNodeAStar(PathNode destinationNode)
        {
            PathNode bestNode = null;
            float bestF = float.MaxValue;

            foreach (PathNode pathNode in openNodes)
            {
                float g = pathNode.AccumulatedCost;
                float h = (destinationNode.Position - pathNode.Position).sqrMagnitude;
                float f = g + h;

                if (f < bestF)
                {
                    bestF = f;
                    bestNode = pathNode;
                }
            }

            return bestNode;
        }

        private Stack<PathNode> GeneratePath (PathNode destinationNode)
        {
            Stack<PathNode> path = new Stack<PathNode>();

            PathNode currentNode = destinationNode;

            while (currentNode != null)
            {
                path.Push(currentNode);
                currentNode = currentNode.Parent;
            }

            return path;
        }

        public Stack<PathNode> CreatePath (Vector3 origin, Vector3 destination)
        {
            _destinationNodeCache = FindClosestNode(destination);

            Stack<PathNode> path = null;

            PathNode originNode = FindClosestNode(origin);
            PathNode destinationNode = FindClosestNode(destination);

            OpenNode(originNode);

            while (openNodes.Count > 0 && path == null)
            {
                PathNode openNode = GetNextOpenNode(destinationNode);

                if (openNode == destinationNode)
                    path = GeneratePath(destinationNode);
                else
                    OpenAdjacentNodes(openNode);

                CloseNode(openNode);
            }

            ResetNodes();

            return path;
        }
    }
}