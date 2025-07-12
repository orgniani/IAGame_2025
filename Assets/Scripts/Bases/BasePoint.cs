using Miners;
using UnityEngine;

namespace Bases
{
    public class BasePoint : MonoBehaviour
    {
        private BaseManager _baseManager;
        public bool IsOccupied { get; private set; }
        public Miner ReservedBy { get; private set; }
        public Vector3 Position => transform.position;

        public void SetManager(BaseManager manager)
        {
            _baseManager = manager;
        }

        public bool TryReserve(Miner miner)
        {
            if (IsOccupied)
                return false;

            IsOccupied = true;
            ReservedBy = miner;
            Debug.Log($"[Base] {name} reserved by {miner.name}");
            return true;
        }

        public void ReleaseReservation(Miner miner)
        {
            if (ReservedBy == miner)
            {
                IsOccupied = false;
                ReservedBy = null;
                Debug.Log($"[Base] {name} released by {miner.name}");
            }
        }

        private void OnDestroy()
        {
            _baseManager?.UnregisterBasePoint(this);
        }
    }
}
