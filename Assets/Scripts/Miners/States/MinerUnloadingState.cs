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
            _coroutineHost = owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerUnloadingState)} on {owner.name}");

            _coroutineHost.StartCoroutine(UnloadRoutine());
        }

        public override void Update() { }

        public override void Exit() { Debug.Log($"[FSM] Exiting {nameof(MinerUnloadingState)} on {owner.name}"); }

        private IEnumerator UnloadRoutine()
        {
            yield return new WaitForSeconds(unloadTime);

            owner.ReportUnloadedGold();        
            owner.Inventory.Clear();
            owner.UpdateBillboard();

            owner.OnUnloadFinished?.Invoke();
        }
    }
}