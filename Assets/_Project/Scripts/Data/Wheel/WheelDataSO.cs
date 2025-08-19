using UnityEngine;
using _Project.Scripts.Data.Reward;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Data.Wheel
{
    [CreateAssetMenu(fileName = "WheelDataSO", menuName = "Project/Wheel/WheelDataSO", order = 1)]
    public class WheelDataSO : ScriptableObject
    {
        [Header("Basic Settings")] 
        [SerializeField] private WheelVisualConfig _wheelVisualConfig;
        [SerializeField] private RewardData[] _rewardDataPool;
        
        [Header("Bomb Settings")]
        [SerializeField] private bool _hasBomb = true;
        
        public bool HasRewards => _rewardDataPool?.Length > 0;
        public WheelType Type => _wheelVisualConfig.Type; 
        public WheelVisualConfig VisualConfig => _wheelVisualConfig;
        public bool HasBomb => _hasBomb;

        public RewardData GetRandomRewardData()
        {
            RewardData randomReward = _rewardDataPool[Random.Range(0, _rewardDataPool.Length)];
            while (randomReward.RewardItemSo.Type == RewardType.Bomb)
            {
                randomReward = _rewardDataPool[Random.Range(0, _rewardDataPool.Length)];
            }
            return randomReward;
        }
    }
}