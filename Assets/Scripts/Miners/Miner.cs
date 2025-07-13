using UnityEngine;
using UnityEngine.Events;
using StateMachine;
using Nodes;
using Mines;
using UI;
using Bases;
using Managers;

namespace Miners
{
    [RequireComponent(typeof(PathNodeAgent), typeof(MinerInventory))]
    public class Miner : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] private MinerIdleState idleState = new MinerIdleState();
        [SerializeField] private MinerGoingToMineState goingToMineState = new MinerGoingToMineState();
        [SerializeField] private MinerMiningState miningState = new MinerMiningState();
        [SerializeField] private MinerReturningState returningState = new MinerReturningState();
        [SerializeField] private MinerUnloadingState unloadingState = new MinerUnloadingState();

        [Header("Common References")]
        [SerializeField] private MineManager mineManager;
        [SerializeField] private BaseManager baseManager;

        [Header("Mining Settings")]
        [SerializeField, Range(0.1f, 10f)] private float miningEfficiency = 1f;

        [Header("UI")]
        [SerializeField] private UIBillboard uiBillboard;

        private FiniteStateMachine<Miner> _fsm;
        private MinerInventory _inventory;

        private GoldMine _currentTargetMine;
        private BasePoint _currentBase;

        private bool _isWaitingForMine = false;

        public GoldMine CurrentMine => _currentTargetMine;
        public MineManager MineManager => mineManager;
        public float MiningEfficiency => miningEfficiency;
        public MinerInventory Inventory => _inventory;
        public BasePoint CurrentBase => _currentBase;
        public bool IsInitialized { get; private set; } = false;

        public UnityEvent OnStartMovingToMine = new UnityEvent();
        public UnityEvent OnReachedMine = new UnityEvent();
        public UnityEvent OnStartReturning = new UnityEvent();
        public UnityEvent OnReachedBase = new UnityEvent();
        public UnityEvent OnUnloadFinished = new UnityEvent();

        private void Awake()
        {
            _inventory = GetComponent<MinerInventory>();

            idleState.Initialize(this);
            goingToMineState.Initialize(this);
            miningState.Initialize(this);
            returningState.Initialize(this);
            unloadingState.Initialize(this);

            UpdateBillboard();
        }

        private void OnEnable()
        {
            PathfindingManager.Instance.OnStrategyChanged += OnPathfindingStrategyChanged;
        }

        private void OnDisable()
        {
            if (PathfindingManager.Instance)
                PathfindingManager.Instance.OnStrategyChanged -= OnPathfindingStrategyChanged;
        }

        private void Start()
        {
            FsmState<Miner>[] states = {
                idleState,
                goingToMineState,
                miningState,
                returningState,
                unloadingState
            };

            UnityEvent[] events = {
            OnStartMovingToMine,
            OnReachedMine,
            OnStartReturning,
            OnReachedBase,
            OnUnloadFinished
            };

            _fsm = new FiniteStateMachine<Miner>(states, events, idleState);

            _fsm.ConfigureTransition(idleState, goingToMineState, OnStartMovingToMine);
            _fsm.ConfigureTransition(miningState, goingToMineState, OnStartMovingToMine);
            
            _fsm.ConfigureTransition(goingToMineState, miningState, OnReachedMine);
            
            _fsm.ConfigureTransition(miningState, returningState, OnStartReturning);
            _fsm.ConfigureTransition(returningState, unloadingState, OnReachedBase);
            _fsm.ConfigureTransition(unloadingState, idleState, OnUnloadFinished);

            IsInitialized = true;
        }

        private void Update()
        {
            _fsm.Update();
        }

        public void SelectNewMine()
        {
            if (_currentTargetMine != null || _isWaitingForMine)
                return;

            _isWaitingForMine = true;

            var newMine = mineManager.GetBestAvailableMine(transform.position, this);

            if (newMine != null)
            {
                _currentTargetMine = newMine;
                Debug.Log($"[Miner] {name} successfully reserved {newMine.name}");
            }
            else
            {
                Debug.LogWarning($"[Miner] {name} could not find a valid mine.");
            }

            _isWaitingForMine = false;
        }

        public void ClearCurrentMine()
        {
            if (_currentTargetMine != null)
            {
                _currentTargetMine.ReleaseReservation(this);
                Debug.Log($"[Miner] {name} released {_currentTargetMine.name}");
                _currentTargetMine = null;
            }

            _isWaitingForMine = false;
        }

        public void SelectNewBase()
        {
            if (_currentBase != null)
                return;

            var newBase = baseManager.GetBestAvailableBase(transform.position, this);
            if (newBase != null)
            {
                _currentBase = newBase;
                Debug.Log($"[Miner] {name} reserved base {newBase.name}");
            }
            else
            {
                Debug.LogWarning($"[Miner] {name} could not find an available base.");
            }
        }

        public void ClearCurrentBase()
        {
            if (_currentBase != null)
            {
                _currentBase.ReleaseReservation(this);
                Debug.Log($"[Miner] {name} released base {_currentBase.name}");
                _currentBase = null;
            }
        }

        public void UpdateBillboard()
        {
            if (uiBillboard != null)
                uiBillboard.SetGold(_inventory.CurrentGold);
        }

        public void ReportUnloadedGold()
        {
            mineManager.ReportGoldDeposited(_inventory.CurrentGold);
        }

        private void OnPathfindingStrategyChanged()
        {
            if (!IsInitialized)
                return;

            if (_fsm.CurrentState == goingToMineState && _currentTargetMine != null)
            {
                var agent = GetComponent<PathNodeAgent>();
                agent.Destination = _currentTargetMine.Position;

                Debug.Log($"[Miner] {name} recalculated path to mine due to strategy change");
            }

            else if (_fsm.CurrentState == returningState && _currentBase != null)
            {
                var agent = GetComponent<PathNodeAgent>();
                agent.Destination = _currentBase.Position;

                Debug.Log($"[Miner] {name} recalculated path to base due to strategy change");
            }
        }
    }
}