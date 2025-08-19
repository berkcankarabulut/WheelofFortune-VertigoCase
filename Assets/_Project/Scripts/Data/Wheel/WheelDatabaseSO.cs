using System.Linq; 
using UnityEngine;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Data.Wheel
{
    [CreateAssetMenu(fileName = "WheelDatabaseSO", menuName = "Project/Wheel/WheelDatabaseSO")]
    public class WheelDatabaseSO : ScriptableObject
    {
        [Header("Wheel Configurations")]
        [SerializeField] private WheelDataSO[] _wheels;
        
        [Header("Global Bomb Settings")]
        [SerializeField] private RewardData _globalBombReward;
        [Tooltip("The bomb reward used across all wheels")]

        public WheelDataSO GetByType(WheelType type)
        {
            return _wheels.FirstOrDefault(x => x.Type == type);
        }
        
        public RewardData GetBombReward()
        {
            return _globalBombReward;
        }
    }
}