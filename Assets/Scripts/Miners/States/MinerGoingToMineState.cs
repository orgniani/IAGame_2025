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
            _agent = _owner.GetComponent<PathNodeAgent>();
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerGoingToMineState)} on {_owner.name}");

            _owner.ClearCurrentBase();
            _agent.MovementSpeed = moveSpeed;

            if (_owner.CurrentMine != null)
                _agent.Destination = _owner.CurrentMine.Position;
        }

        public override void Update()
        {
            if (!IsMineValid()) return;

            if (_agent.HasReachedDestination)
            {
                Debug.Log($"[FSM] {_owner.name} reached the mine");
                _owner.OnReachedMine.Invoke();
            }
        }

        public override void Exit() { Debug.Log($"[FSM] Exiting {nameof(MinerGoingToMineState)} on {_owner.name}"); }

        private bool IsMineValid()
        {
            var mine = _owner.CurrentMine;

            if (mine == null || mine.IsDepleted || mine.ReservedBy != _owner)
            {
                Debug.Log($"[FSM] {_owner.name} lost their mine en route");

                _owner.SelectNewMine();

                if (_owner.CurrentMine != null)
                {
                    _agent.Destination = _owner.CurrentMine.Position;
                    Debug.Log($"[FSM] {_owner.name} switching to new mine: {_owner.CurrentMine.name}");
                }
                else _owner.OnStartReturning.Invoke();

                return false;
            }

            return true;
        }
    }
}