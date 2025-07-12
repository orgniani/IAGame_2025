using Managers;
using Miners;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Mines
{
    public class MineManager : MonoBehaviour
    {
        [SerializeField] private PathfindingManager pathfindingManager;
        [SerializeField] private List<GoldMine> goldMines = new();

        [SerializeField] private UIHUD hud;
        private int _totalGold = 0;

        private void Awake()
        {
            foreach (var mine in goldMines)
            {
                if (mine != null)
                    RegisterMine(mine);
            }
        }

        public void ReportGoldDeposited(int amount)
        {
            _totalGold += amount;
            hud?.SetTotalGold(_totalGold);
        }

        public GoldMine GetBestAvailableMine(Vector3 minerPosition, Miner requester)
        {
            GoldMine bestMine = null;
            int bestPathLength = int.MaxValue;

            foreach (var mine in goldMines)
            {
                if (mine == null || mine.IsDepleted || mine.IsOccupied || mine.ReservedBy == requester)
                    continue;

                var path = pathfindingManager.CreatePath(minerPosition, mine.Position);
                if (path == null)
                    continue;

                int pathLength = path.Count;

                if (pathLength < bestPathLength)
                {
                    bestMine = mine;
                    bestPathLength = pathLength;
                }
            }

            if (bestMine != null && bestMine.TryReserve(requester))
                return bestMine;

            return null;
        }

        public bool AllMinesDepleted()
        {
            if (goldMines.Count == 0)
                return true;

            return false;
        }

        public void RegisterMine(GoldMine mine)
        {
            if (!goldMines.Contains(mine))
                goldMines.Add(mine);

            mine.SetManager(this);
        }

        public void UnregisterMine(GoldMine mine)
        {
            if (goldMines.Contains(mine))
                goldMines.Remove(mine);
        }
    }
}