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
            _coroutineHost = _owner;
        }

        public override void Enter()
        {
            Debug.Log($"[FSM] Entering {nameof(MinerIdleState)} on {_owner.name}");

            _owner.SelectNewMine();

            if (_owner.CurrentMine != null)
            {
                Debug.Log($"[FSM] {_owner.name} found mine immediately");
                _retryCoroutine = _coroutineHost.StartCoroutine(SearchForMine());
            }
            else if (_owner.MineManager.AllMinesDepleted())
            {
                Debug.Log($"[FSM] {_owner.name} all mines depleted, going to base");
                _owner.OnStartReturning?.Invoke();
            }
        }

        public override void Update() { }

        public override void Exit()
        {
            if (_retryCoroutine != null)
                _coroutineHost.StopCoroutine(_retryCoroutine);

            _retryCoroutine = null;

            Debug.Log($"[FSM] Exiting {nameof(MinerIdleState)} on {_owner.name}");
        }

        private IEnumerator SearchForMine()
        {
            yield return new WaitUntil(() => _owner.IsInitialized);

            while (!_owner.MineManager.AllMinesDepleted())
            {
                yield return new WaitForSeconds(waitTime);

                _owner.SelectNewMine();

                if (_owner.CurrentMine != null)
                {
                    Debug.Log($"[FSM] {_owner.name} found mine on retry");
                    _owner.OnStartMovingToMine?.Invoke();
                    yield break;
                }

                Debug.Log($"[FSM] {_owner.name} still no mine, retrying...");
            }

            Debug.Log($"[FSM] {_owner.name} all mines depleted after retries");
            _owner.OnStartReturning?.Invoke();
        }
    }
}