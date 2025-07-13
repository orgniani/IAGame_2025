using System;
using UnityEngine;
using StateMachine;
using System.Collections;

namespace Miners
{
    [Serializable]
    public class MinerUnloadingState : FsmState<Miner>
    {
        [SerializeField] private float unloadTime = 2f;

        private MonoBehaviour _coroutineHost;

        protected override void OnInitialize()
        {
            _coroutineHost = _owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerUnloadingState)} on {_owner.name}");

            _coroutineHost.StartCoroutine(UnloadRoutine());
        }

        public override void Update() { }

        public override void Exit() { Debug.Log($"[FSM] Exiting {nameof(MinerUnloadingState)} on {_owner.name}"); }

        private IEnumerator UnloadRoutine()
        {
            yield return new WaitForSeconds(unloadTime);

            _owner.ReportUnloadedGold();        
            _owner.Inventory.Clear();
            _owner.UpdateBillboard();

            _owner.OnUnloadFinished?.Invoke();
        }
    }
}