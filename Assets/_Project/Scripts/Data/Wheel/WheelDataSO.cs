using UnityEngine;
using _Project.Scripts.Data.Reward; 

namespace _Project.Scripts.Data.Wheel
{
    // Wheel'ların data'sını tutan ScriptableObject
    [CreateAssetMenu(fileName = "WheelDataSO", menuName = "Project/Wheel/WheelDataSO", order = 1)]
    public class WheelDataSO : ScriptableObject
    {
        [Header("Basic Settings")] 
        [SerializeField] private WheelVisualConfig _wheelVisualConfig;
        [SerializeField] private RewardData[] _rewardDataPool; 
        public bool HasRewards => RewardDataPool?.Length > 0;
        public WheelType Type => _wheelVisualConfig.Type; 
        public WheelVisualConfig VisualConfig => _wheelVisualConfig;

        public RewardData[] RewardDataPool => _rewardDataPool;
    }
}