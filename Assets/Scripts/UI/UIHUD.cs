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

        private void OnEnable()
        {
            
        }

        public void SetTotalGold(int amount)
        {
            if (totalGoldText != null)
                totalGoldText.text = $"Total gold: {amount.ToString()}";
        }

    }
}