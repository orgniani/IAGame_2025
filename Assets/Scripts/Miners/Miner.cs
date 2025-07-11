using UnityEngine;
using UnityEngine.Events;
using StateMachine;
using Nodes;
using Mines;
using UI;

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

        public GoldMine CurrentMine => currentTargetMine;
        public float MiningEfficiency => miningEfficiency;
        public MinerInventory Inventory => inventory;
        public Transform BasePosition => basePosition;

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
            SelectNewMine();

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
            fsm.ConfigureTransition(goingToMineState, miningState, OnReachedMine);
            fsm.ConfigureTransition(miningState, returningState, OnStartReturning);
            fsm.ConfigureTransition(returningState, unloadingState, OnReachedBase);
            fsm.ConfigureTransition(unloadingState, idleState, OnUnloadFinished);
        }

        void Update()
        {
            fsm.Update();
        }

        public void SelectNewMine()
        {
            if (currentTargetMine != null)
                currentTargetMine.SetOccupied(false);

            currentTargetMine = mineManager.GetBestAvailableMine(transform.position);
            if (currentTargetMine != null)
                currentTargetMine.SetOccupied(true);
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