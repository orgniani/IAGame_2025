using Helpers;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHUD : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text totalGoldText;

        [Header("Buttons")]
        [SerializeField] private Button depthFirstButton;
        [SerializeField] private Button breathFirstButton;
        [SerializeField] private Button dijkstraButton;
        [SerializeField] private Button aStarButton;

        private PathfindingManager _pathfindingManager;

        private void Awake()
        {
            _pathfindingManager = PathfindingManager.Instance;
            ValidateReferences();
        }

        private void OnEnable()
        {
            depthFirstButton.onClick.AddListener(() => SetStrategy(PathfindingManager.PathfindingStrategy.DepthFirst));
            breathFirstButton.onClick.AddListener(() => SetStrategy(PathfindingManager.PathfindingStrategy.BreadthFirst));
            dijkstraButton.onClick.AddListener(() => SetStrategy(PathfindingManager.PathfindingStrategy.Dijkstra));
            aStarButton.onClick.AddListener(() => SetStrategy(PathfindingManager.PathfindingStrategy.AStar));
        }

        private void OnDisable()
        {
            depthFirstButton.onClick.RemoveAllListeners();
            breathFirstButton.onClick.RemoveAllListeners();
            dijkstraButton.onClick.RemoveAllListeners();
            aStarButton.onClick.RemoveAllListeners();
        }

        private void SetStrategy(PathfindingManager.PathfindingStrategy strategy)
        {
            if (_pathfindingManager != null)
            {
                _pathfindingManager.SetStrategy(strategy);
                Debug.Log($"[UIHUD] Strategy set to {strategy}");
            }

            else
            {
                Debug.LogError("[UIHUD] PathfindingManager is not initialized.");
            }
        }

        public void SetTotalGold(int amount)
        {
            if (totalGoldText != null)
                totalGoldText.text = $"Total gold: {amount.ToString()}";
        }


        private void ValidateReferences()
        {
            ReferenceValidator.Validate(_pathfindingManager, nameof(_pathfindingManager), this);
            ReferenceValidator.Validate(totalGoldText, nameof(totalGoldText), this);

            ReferenceValidator.Validate(depthFirstButton, nameof(depthFirstButton), this);
            ReferenceValidator.Validate(breathFirstButton, nameof(breathFirstButton), this);
            ReferenceValidator.Validate(dijkstraButton, nameof(dijkstraButton), this);
            ReferenceValidator.Validate(aStarButton, nameof(aStarButton), this);
        }
    }
}