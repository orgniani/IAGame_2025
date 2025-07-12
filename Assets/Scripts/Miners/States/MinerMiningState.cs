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

            while (CanContinueMining(mine))
            {
                yield return new WaitForSeconds(secondsPerCycle);
                MineGold(mine);
            }

            HandlePostMining(mine);
        }

        private bool CanContinueMining(GoldMine mine)
        {
            return mine != null && !mine.IsDepleted && !owner.Inventory.IsFull;
        }

        private void MineGold(GoldMine mine)
        {
            int extracted = mine.ExtractGold(goldPerCycle);
            owner.Inventory.AddGold(extracted);

            owner.UpdateBillboard();

            Debug.Log($"[FSM] {owner.name} mined {extracted}, now carrying {owner.Inventory.CurrentGold}");
        }

        private void HandlePostMining(GoldMine mine)
        {
            owner.ClearCurrentMine();

            if (owner.Inventory.IsFull)
            {
                owner.OnStartReturning?.Invoke();
                return;
            }

            owner.SelectNewMine();

            if (owner.CurrentMine != null)
                owner.OnStartMovingToMine?.Invoke();
            else
                owner.OnStartReturning?.Invoke();
        }
    }
}