using System;
using UnityEngine;
using StateMachine;
using Nodes;

namespace Miners
{
    [Serializable]
    public class MinerReturningState : FsmState<Miner>
    {
        [SerializeField] private float returnSpeed = 3f;

        private PathNodeAgent _agent;

        protected override void OnInitialize()
        {
            _agent = owner.GetComponent<PathNodeAgent>();
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerReturningState)} on {owner.name}");

            _agent.MovementSpeed = returnSpeed;
            _agent.Destination = owner.BasePosition.position;
        }

        public override void Update()
        {
            if (_agent.HasReachedDestination)
            {
                Debug.Log($"[FSM] {owner.name} returned to base");
                owner.OnReachedBase.Invoke();
            }
        }

        public override void Exit() { Debug.Log($"[FSM] Exiting {nameof(MinerReturningState)} on {owner.name}");  }
    }
}