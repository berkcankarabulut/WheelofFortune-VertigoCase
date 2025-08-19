using _Project.Scripts.Config;
using UnityEngine;

namespace _Project.Scripts.Utils
{
    public static class MultiplierUtility
    {
        public static float CalculateZoneMultiplier(int zone, IGameSettings gameSettings)
        {
            if (zone <= 1) return 1f;

            float multiplier = Mathf.Pow(gameSettings.ZoneRewardMultiplier, zone - 1);

            if (zone % gameSettings.SuperZoneInterval == 0)
            {
                multiplier *= gameSettings.SuperZoneMultiplier;
            }

            return multiplier;
        }

        public static int ApplyMultiplierToReward(int baseAmount, float multiplier, int maxLimit)
        {
            int scaledAmount = Mathf.RoundToInt(baseAmount * multiplier);
            return Mathf.Clamp(scaledAmount, 0, maxLimit);
        }
    }
}