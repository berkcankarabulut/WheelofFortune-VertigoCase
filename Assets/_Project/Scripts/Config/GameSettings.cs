using UnityEngine;

namespace _Project.Scripts.Config
{
    public static class GameSettings
    {
        public const int SafeZoneInterval = 5;
        public const int SuperZoneInterval = 30;
        public const int RevivePrice = 25;
        
        [Header("Zone Reward Multipliers")]
        public const float ZoneRewardMultiplier = 1.2f;  
        public const float SuperZoneMultiplier = 2.0f;  
    }
}