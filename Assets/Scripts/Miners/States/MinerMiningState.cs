using System;
using UnityEngine;
using StateMachine;
using System.Collections;

namespace Miners
{
    [Serializable]
    public class MinerMiningState : FsmState<Miner>
    {
        [SerializeField] private int goldPerCycle = 1;
        [SerializeField] private float secondsPerCycle = 1f;

        private MonoBehaviour _coroutineHost;
        private Coroutine _miningRoutine;

        protected override void OnInitialize()
        {
            _coroutineHost = owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerMiningState)} on {owner.name}");

            if (owner.CurrentMine == null)
            {
                Debug.LogWarning($"[FSM] {owner.name} tried to mine but has no target mine.");
                owner.OnStartReturning.Invoke();
                return;
            }

            _miningRoutine = _coroutineHost.StartCoroutine(MiningLoop());
        }

        public override void Exit()
        {
            Debug.Log($"[FSM] Exiting {nameof(MinerMiningState)} on {owner.name}");

            if (_miningRoutine != null)
                _coroutineHost.StopCoroutine(_miningRoutine);
        }

        public override void Update() { }

        private IEnumerator MiningLoop()
        {
            var mine = owner.CurrentMine;

            while (!owner.Inventory.IsFull && mine != null && !mine.IsDepleted)
            {
                yield return new WaitForSeconds(secondsPerCycle);

                int extracted = mine.ExtractGold(goldPerCycle);
                owner.Inventory.AddGold(extracted);

                Debug.Log($"[FSM] {owner.name} mined {extracted}, now carrying {owner.Inventory.CurrentGold}");
            }

            if (mine != null)
                mine.SetOccupied(false);

            owner.OnStartReturning?.Invoke();
        }
    }
}