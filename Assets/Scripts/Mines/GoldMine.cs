using Helpers;
using Miners;
using UnityEngine;

namespace Mines
{
    public class GoldMine : MonoBehaviour
    {
        [SerializeField] private int goldAmount = 10;
        [SerializeField] private GameObject goldVisual;

        private MineManager _mineManager;

        public bool IsOccupied { get; private set; } = false;
        public bool IsDepleted => goldAmount <= 0;
        public Vector3 Position => transform.position;
        public Miner ReservedBy { get; private set; }

        private void Awake()
        {
            ReferenceValidator.Validate(goldVisual, nameof(goldVisual), this);
        }

        public void SetManager(MineManager manager)
        {
            _mineManager = manager;
        }

        public int ExtractGold(int amount)
        {
            int extracted = Mathf.Min(amount, goldAmount);
            goldAmount -= extracted;

            if (IsDepleted)
            {
                _mineManager?.UnregisterMine(this);
                goldVisual.SetActive(false);
            }

            return extracted;
        }

        public bool TryReserve(Miner miner)
        {
            if (IsDepleted || IsOccupied)
                return false;

            IsOccupied = true;
            ReservedBy = miner;

            Debug.Log($"[Mine] {name} reserved by {miner.name}");
            return true;
        }

        public void ReleaseReservation(Miner miner)
        {
            if (ReservedBy == miner)
            {
                IsOccupied = false;
                ReservedBy = null;
                Debug.Log($"[Mine] {name} released by {miner.name}");
            }
        }

        void OnDestroy()
        {
            _mineManager?.UnregisterMine(this);
        }
    }
}