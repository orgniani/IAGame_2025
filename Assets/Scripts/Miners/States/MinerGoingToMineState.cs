using System;
using UnityEngine;
using StateMachine;
using Nodes;

namespace Miners
{
    [Serializable]
    public class MinerGoingToMineState : FsmState<Miner>
    {
        [SerializeField] private float moveSpeed = 3f;

        private PathNodeAgent _agent;

        protected override void OnInitialize()
        {
            _agent = owner.GetComponent<PathNodeAgent>();
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerGoingToMineState)} on {owner.name}");

            _agent.MovementSpeed = moveSpeed;

            if (owner.CurrentMine != null)
                _agent.Destination = owner.CurrentMine.Position;
        }

        public override void Update()
        {
            if (_agent.HasReachedDestination)
            {
                Debug.Log($"[FSM] {owner.name} reached the mine");
                owner.OnReachedMine.Invoke();
            }
        }

        public override void Exit() { Debug.Log($"[FSM] Exiting {nameof(MinerGoingToMineState)} on {owner.name}"); }
    }
}