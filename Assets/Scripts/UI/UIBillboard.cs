using Helpers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIBillboard : MonoBehaviour
    {
        [SerializeField] private TMP_Text collectedGoldText;
        private Camera _mainCamera;

        private void Awake()
        {
            ReferenceValidator.Validate(collectedGoldText, nameof(collectedGoldText), this);
        }
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera != null)
                transform.forward = _mainCamera.transform.forward;
        }

        public void SetGold(int goldAmount)
        {
            if (collectedGoldText != null)
                collectedGoldText.text = $"Gold: {goldAmount.ToString()}";
        }
    }
}