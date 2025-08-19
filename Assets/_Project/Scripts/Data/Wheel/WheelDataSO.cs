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
        
        public bool HasRewards => _rewardDataPool?.Length > 0;
        public WheelType Type => _wheelVisualConfig.Type; 
        public WheelVisualConfig VisualConfig => _wheelVisualConfig;

        public RewardData GetRandomRewardData()
        {
            RewardData randomRewardData = _rewardDataPool[Random.Range(0, _rewardDataPool.Length)];
            while (randomRewardData.RewardItemSo.Type == RewardType.Bomb)
            {
                randomRewardData = _rewardDataPool[Random.Range(0, _rewardDataPool.Length)];
            }
            return randomRewardData;
        }

        public RewardData GetBombRewardData()
        { 
            for (int i = 0; i < _rewardDataPool.Length; i++)
            {
                if (_rewardDataPool[i].RewardItemSo.Type == RewardType.Bomb)
                {
                    return _rewardDataPool[i];
                }
            }
             
            Debug.LogWarning($"No bomb reward found in {name}");
            return null;
        }

        public bool HasBombReward()
        {
            for (int i = 0; i < _rewardDataPool.Length; i++)
            {
                if (_rewardDataPool[i].RewardItemSo.Type == RewardType.Bomb)
                {
                    return true;
                }
            }
            return false;
        }
    }
}