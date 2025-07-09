using UnityEngine;

namespace Miners
{
    public class MinerInventory : MonoBehaviour
    {
        [SerializeField] private int maxCapacity = 5;
        public int CurrentGold { get; private set; } = 0;
        public bool IsFull => CurrentGold >= maxCapacity;

        public void AddGold(int amount)
        {
            CurrentGold += amount;
            CurrentGold = Mathf.Min(CurrentGold, maxCapacity);
        }

        public void Clear() => CurrentGold = 0;
    }
}
