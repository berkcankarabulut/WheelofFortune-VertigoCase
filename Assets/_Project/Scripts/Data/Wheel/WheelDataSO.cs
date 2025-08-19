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
        [SerializeField] private ItemAmountData[] _rewardDataPool;
        
        public bool HasRewards => _rewardDataPool?.Length > 0;
        public WheelType Type => _wheelVisualConfig.Type; 
        public WheelVisualConfig VisualConfig => _wheelVisualConfig;

        public ItemAmountData GetRandomRewardData()
        {
            ItemAmountData randomItemAmountData = _rewardDataPool[Random.Range(0, _rewardDataPool.Length)];
            while (randomItemAmountData.ItemSo.Type == RewardType.Bomb)
            {
                randomItemAmountData = _rewardDataPool[Random.Range(0, _rewardDataPool.Length)];
            }
            return randomItemAmountData;
        }

        public ItemAmountData GetBombRewardData()
        { 
            for (int i = 0; i < _rewardDataPool.Length; i++)
            {
                if (_rewardDataPool[i].ItemSo.Type == RewardType.Bomb)
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
                if (_rewardDataPool[i].ItemSo.Type == RewardType.Bomb)
                {
                    return true;
                }
            }
            return false;
        }
    }
}