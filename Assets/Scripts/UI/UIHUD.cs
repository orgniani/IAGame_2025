using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text totalGoldText;

        public void SetTotalGold(int amount)
        {
            if (totalGoldText != null)
                totalGoldText.text = $"Total gold: {amount.ToString()}";
        }

    }
}