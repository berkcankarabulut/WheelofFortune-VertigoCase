using UnityEngine;

namespace _Project.Scripts.Config
{ 
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Project/Config/GameSettings")]
    public class GameSettingsSO : ScriptableObject, IGameSettings
    {
        [Header("Zone Settings")]
        [SerializeField] private int _safeZoneInterval = 5;
        [SerializeField] private int _superZoneInterval = 30;
        [SerializeField] private int _revivePrice = 25;
        
        [Header("Zone Reward Multipliers")]
        [SerializeField] private float _zoneRewardMultiplier = 1.2f;
        [SerializeField] private float _zoneSafeMultiplier = 1.2f;
        [SerializeField] private float _superZoneMultiplier = 2.0f;
        
        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;
        public int RevivePrice => _revivePrice;
        public float ZoneRewardMultiplier => _zoneRewardMultiplier;
        public float SafeRewardMultiplier=> _zoneSafeMultiplier;  
        public float SuperZoneMultiplier => _superZoneMultiplier; 
    }
}