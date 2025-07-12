using System;
using UnityEngine;
using StateMachine;
using System.Collections;

namespace Miners
{
    [Serializable]
    public class MinerIdleState : FsmState<Miner>
    {
        [SerializeField] private float waitTime = 2f;

        private MonoBehaviour _coroutineHost;

        protected override void OnInitialize()
        {
            _coroutineHost = owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerIdleState)} on {owner.name}");
            _coroutineHost.StartCoroutine(WaitThenGoToMine());
        }

        public override void Update() { }

        public override void Exit() { Debug.Log($"[FSM] Exiting {nameof(MinerIdleState)} on {owner.name}"); }

        private IEnumerator WaitThenGoToMine()
        {
            yield return new WaitForSeconds(waitTime);

            owner.SelectNewMine();

            if (owner.CurrentMine != null)
                owner.OnStartMovingToMine.Invoke();
            else
                owner.OnStartReturning.Invoke();
        }
    }
}