using UnityEngine;
using UnityEngine.Events;
using StateMachine;
using Nodes;
using Mines;
using UI;
using static UnityEngine.UI.GridLayoutGroup;

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
        [SerializeField] private Transform basePosition;

        [Header("Mining Settings")]
        [SerializeField, Range(0.1f, 10f)] private float miningEfficiency = 1f;

        [Header("UI")]
        [SerializeField] private UIBillboard uiBillboard;

        private FiniteStateMachine<Miner> fsm;
        private MinerInventory inventory;
        private GoldMine currentTargetMine;
        private bool _isWaitingForMine = false;

        public GoldMine CurrentMine => currentTargetMine;
        public MineManager MineManager => mineManager;
        public float MiningEfficiency => miningEfficiency;
        public MinerInventory Inventory => inventory;
        public Transform BasePosition => basePosition;
        public bool IsInitialized { get; private set; } = false;

        public UnityEvent OnStartMovingToMine = new UnityEvent();
        public UnityEvent OnReachedMine = new UnityEvent();
        public UnityEvent OnStartReturning = new UnityEvent();
        public UnityEvent OnReachedBase = new UnityEvent();
        public UnityEvent OnUnloadFinished = new UnityEvent();

        void Awake()
        {
            inventory = GetComponent<MinerInventory>();

            idleState.Initialize(this);
            goingToMineState.Initialize(this);
            miningState.Initialize(this);
            returningState.Initialize(this);
            unloadingState.Initialize(this);

            UpdateBillboard();
        }

        void Start()
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

            fsm = new FiniteStateMachine<Miner>(states, events, idleState);

            fsm.ConfigureTransition(idleState, goingToMineState, OnStartMovingToMine);
            fsm.ConfigureTransition(miningState, goingToMineState, OnStartMovingToMine);
            
            fsm.ConfigureTransition(goingToMineState, miningState, OnReachedMine);
            
            fsm.ConfigureTransition(miningState, returningState, OnStartReturning);
            fsm.ConfigureTransition(returningState, unloadingState, OnReachedBase);
            fsm.ConfigureTransition(unloadingState, idleState, OnUnloadFinished);

            IsInitialized = true;
        }

        void Update()
        {
            fsm.Update();
        }

        public void SelectNewMine()
        {
            if (currentTargetMine != null || _isWaitingForMine)
                return;

            _isWaitingForMine = true;

            var newMine = mineManager.GetBestAvailableMine(transform.position, this);

            if (newMine != null)
            {
                currentTargetMine = newMine;
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
            if (currentTargetMine != null)
            {
                currentTargetMine.ReleaseReservation(this);
                Debug.Log($"[Miner] {name} released {currentTargetMine.name}");
                currentTargetMine = null;
            }

            _isWaitingForMine = false;
        }


        public void UpdateBillboard()
        {
            if (uiBillboard != null)
                uiBillboard.SetGold(inventory.CurrentGold);
        }

        public void ReportUnloadedGold()
        {
            mineManager.ReportGoldDeposited(inventory.CurrentGold);
        }
    }
}