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
        private Coroutine _retryCoroutine;

        protected override void OnInitialize()
        {
            _coroutineHost = owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerIdleState)} on {owner.name}");

            owner.SelectNewMine();

            if (owner.CurrentMine != null)
            {
                Debug.Log($"[FSM] {owner.name} found mine immediately");
                _retryCoroutine = _coroutineHost.StartCoroutine(RetryMineSearchLoop());
            }
            else if (owner.MineManager.AllMinesDepleted())
            {
                Debug.Log($"[FSM] {owner.name} all mines depleted, going to base");
                owner.OnStartReturning?.Invoke();
            }
        }

        public override void Update() { }

        public override void Exit()
        {
            if (_retryCoroutine != null)
                _coroutineHost.StopCoroutine(_retryCoroutine);

            _retryCoroutine = null;

            Debug.Log($"[FSM] Exiting {nameof(MinerIdleState)} on {owner.name}");
        }

        private IEnumerator RetryMineSearchLoop()
        {
            yield return new WaitUntil(() => owner.IsInitialized);

            while (!owner.MineManager.AllMinesDepleted())
            {
                yield return new WaitForSeconds(waitTime);

                owner.SelectNewMine();

                if (owner.CurrentMine != null)
                {
                    Debug.Log($"[FSM] {owner.name} found mine on retry");
                    owner.OnStartMovingToMine?.Invoke();
                    yield break;
                }

                Debug.Log($"[FSM] {owner.name} still no mine, retrying...");
            }

            Debug.Log($"[FSM] {owner.name} all mines depleted after retries");
            owner.OnStartReturning?.Invoke();
        }
    }
}