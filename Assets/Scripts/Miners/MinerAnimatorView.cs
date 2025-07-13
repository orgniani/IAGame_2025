using Bases;
using Helpers;
using Mines;
using UI;
using UnityEngine;

namespace Miners
{
    [RequireComponent(typeof(Animator))]
    public class MinerAnimatorView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Miner miner;

        [Header("Animator Parameters")]
        [SerializeField] private string horSpeedParameter = "hor_speed";
        [SerializeField] private string miningBoolParameter = "mining";
        
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            ValidateReferences();
        }

        private void OnEnable()
        {
            miner.OnReachedMine.AddListener(HandleMining);

            miner.OnStartReturning.AddListener(HandleStopMining);
            miner.OnStartMovingToMine.AddListener(HandleStopMining);
        }

        private void OnDisable()
        {
            miner.OnReachedMine.RemoveListener(HandleMining);

            miner.OnStartReturning.RemoveListener(HandleStopMining);
            miner.OnStartMovingToMine.RemoveListener(HandleStopMining);
        }

        private void HandleMining()
        {
            _animator.SetBool(miningBoolParameter, true);
        }

        private void HandleStopMining()
        {
            _animator.SetBool(miningBoolParameter, false);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(miner, nameof(miner), this);
            ReferenceValidator.Validate(_animator, nameof(_animator), this);
        }
    }
}