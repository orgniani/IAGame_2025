using UnityEngine;

namespace Mines
{
    public class GoldMine : MonoBehaviour
    {
        [SerializeField] private MineManager mineManager;
        [SerializeField] private int goldAmount = 10;
        [SerializeField] private GameObject goldVisual;

        public bool IsOccupied { get; private set; } = false;
        public bool IsDepleted => goldAmount <= 0;
        public Vector3 Position => transform.position;

        void Awake()
        {
            mineManager?.RegisterMine(this);
        }

        public void SetOccupied(bool value) => IsOccupied = value;

        public int ExtractGold(int amount)
        {
            int extracted = Mathf.Min(amount, goldAmount);
            goldAmount -= extracted;

            if (IsDepleted)
            {
                mineManager?.UnregisterMine(this);
                goldVisual.SetActive(false);
            }

            return extracted;
        }

        void OnDestroy()
        {
            mineManager?.UnregisterMine(this);
        }
    }
}