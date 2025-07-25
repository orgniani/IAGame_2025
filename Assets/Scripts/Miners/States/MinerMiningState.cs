using System;
using UnityEngine;
using StateMachine;
using System.Collections;
using Mines;

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
            _coroutineHost = _owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerMiningState)} on {_owner.name}");

            if (_owner.CurrentMine == null)
            {
                Debug.LogWarning($"[FSM] {_owner.name} tried to mine but has no target mine.");
                _owner.OnStartReturning.Invoke();
                return;
            }

            _miningRoutine = _coroutineHost.StartCoroutine(MiningLoop());
        }

        public override void Exit()
        {
            Debug.Log($"[FSM] Exiting {nameof(MinerMiningState)} on {_owner.name}");

            if (_miningRoutine != null)
                _coroutineHost.StopCoroutine(_miningRoutine);
        }

        public override void Update() { }

        private IEnumerator MiningLoop()
        {
            var mine = _owner.CurrentMine;

            while (CanContinueMining(mine))
            {
                yield return new WaitForSeconds(secondsPerCycle);
                MineGold(mine);
            }

            HandlePostMining(mine);
        }

        private bool CanContinueMining(GoldMine mine)
        {
            return mine != null && !mine.IsDepleted && !_owner.Inventory.IsFull;
        }

        private void MineGold(GoldMine mine)
        {
            int extracted = mine.ExtractGold(goldPerCycle);
            _owner.Inventory.AddGold(extracted);

            _owner.UpdateBillboard();

            Debug.Log($"[FSM] {_owner.name} mined {extracted}, now carrying {_owner.Inventory.CurrentGold}");
        }

        private void HandlePostMining(GoldMine mine)
        {
            _owner.ClearCurrentMine();

            if (_owner.Inventory.IsFull)
            {
                _owner.OnStartReturning?.Invoke();
                return;
            }

            _owner.SelectNewMine();

            if (_owner.CurrentMine != null)
                _owner.OnStartMovingToMine?.Invoke();
            else
                _owner.OnStartReturning?.Invoke();
        }
    }
}