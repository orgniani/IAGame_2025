using Managers;
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

        public void ReportGoldDeposited(int amount)
        {
            _totalGold += amount;
            hud?.SetTotalGold(_totalGold);
        }

        public GoldMine GetBestAvailableMine(Vector3 minerPosition)
        {
            GoldMine bestMine = null;
            int bestPathLength = int.MaxValue;

            foreach (var mine in goldMines)
            {
                if (mine == null || mine.IsOccupied || mine.IsDepleted)
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

            return bestMine;
        }

        public void RegisterMine(GoldMine mine)
        {
            if (!goldMines.Contains(mine))
                goldMines.Add(mine);
        }

        public void UnregisterMine(GoldMine mine)
        {
            if (goldMines.Contains(mine))
                goldMines.Remove(mine);
        }
    }
}