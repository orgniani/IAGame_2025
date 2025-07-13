using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Nodes
{
    public class PathNodeAgent : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float rotateSpeed = 45f;

        [Header("Idle Settings")]
        [SerializeField] private bool rotateTowardDestinationWhenIdle = true;
        [SerializeField] private float idleRotationSpeed = 360f;

        private Stack<PathNode> _currentPath;
        private PathNode _targetNode;
        private Vector3? _destination;

        private Vector3 _lastPosition;
        private Vector3 _currentVelocity;
        public Vector3 CurrentVelocity => _currentVelocity;

        public bool HasReachedDestination { get; private set; } = false;
        public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }

        public Vector3? Destination 
        { 
            get => _destination;
            set
            {
                _destination = value;

                if (_destination != null)
                {
                    _currentPath = PathfindingManager.Instance.CreatePath(transform.position, _destination.Value);
                    _targetNode = _currentPath.Pop();
                    HasReachedDestination = false;
                }
            }
        }

        void Update()
        {
            if (Destination == null)
            {
                _currentVelocity = Vector3.zero;
                return;
            }

            if (HasReachedDestination)
            {
                _currentVelocity = Vector3.zero;
                LookAtTarget();

                return;
            }

            _currentVelocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            Vector3 targetPosition = _targetNode.Position;
            Vector3 toTarget = targetPosition - transform.position;
            float distanceSqr = toTarget.sqrMagnitude;

            float maxDistanceDelta = movementSpeed * Time.deltaTime;
            float maxDegreesDelta = rotateSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistanceDelta);

            if (distanceSqr > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, transform.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta);
            }

            if (distanceSqr <= 0.01f)
            {
                if (_currentPath.Count > 0)
                    _targetNode = _currentPath.Pop();
                else
                    HasReachedDestination = true;
            }
        }

        private void LookAtTarget()
        {
            if (rotateTowardDestinationWhenIdle && Destination.HasValue)
            {
                Vector3 lookDir = Destination.Value - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, transform.up);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        idleRotationSpeed * Time.deltaTime
                    );
                }
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if (_targetNode == null)
                return;

            float lineThickness = 5f;
            UnityEditor.Handles.color = Color.red;

            UnityEditor.Handles.DrawLine(transform.position, _targetNode.Position, lineThickness);

            if (_currentPath != null && _currentPath.Count > 0)
            {
                PathNode[] nodes = _currentPath.ToArray();

                UnityEditor.Handles.DrawLine(_targetNode.Position, nodes[0].Position, lineThickness);

                for (int i = 0; i < nodes.Length - 1; i++)
                    UnityEditor.Handles.DrawLine(nodes[i].Position, nodes[i + 1].Position, lineThickness);
            }
        }
#endif
    }
}