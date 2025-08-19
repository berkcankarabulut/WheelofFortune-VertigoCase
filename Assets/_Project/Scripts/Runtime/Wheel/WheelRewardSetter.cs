using _Project.Scripts.Data.Reward;
using _Project.Scripts.Service;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Zone; 
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelRewardSetter : MonoBehaviour, IWheelRewardSetter
    {
        [Header("Reward UI References")] [SerializeField]
        private WheelRewardUIElement[] _wheelRewardUIs;

        private IWheelDataService _wheelDataService;
        private MultiplierCalculator _multiplierCalculator;

        [Inject]
        public void Construct(
            IWheelDataService wheelDataService,
            MultiplierCalculator multiplierCalculator)
        {
            _wheelDataService = wheelDataService;
            _multiplierCalculator = multiplierCalculator;
        }

        public void LoadRewards(int zone)
        {
            WheelDataSO wheelData = _wheelDataService.GetConfigsForZone(zone);
            SetRewardsForWheel(wheelData, _wheelDataService.GetBombReward());
        }

        private void SetRewardsForWheel(WheelDataSO wheelData, RewardData bombReward)
        {
            if (!wheelData) return;
            if (!wheelData.HasRewards || _wheelRewardUIs?.Length == 0) return;

            bool shouldAddBomb = wheelData.Type == WheelType.BronzeZone;
            int bombSliceIndex = shouldAddBomb ? Random.Range(0, _wheelRewardUIs.Length) : -1;
 
            float currentMultiplier = _multiplierCalculator.CurrentMultiplier.Value;

            for (int i = 0; i < _wheelRewardUIs.Length; i++)
            {
                bool isBombSlice = i == bombSliceIndex;
                RewardData rewardData = isBombSlice
                    ? bombReward
                    : GetScaledReward(wheelData.GetRandomRewardData(), currentMultiplier);

                _wheelRewardUIs[i]?.SetRewardData(rewardData); 
            }
        }

        private RewardData GetScaledReward(RewardData baseReward, float multiplier)
        {
            if (baseReward == null) return null;

            int scaledAmount = Mathf.RoundToInt(baseReward.Amount * multiplier);
            scaledAmount = Mathf.Clamp(scaledAmount, 0, baseReward.RewardItemSo.MaxLimit);
            return new RewardData(baseReward.RewardItemSo, scaledAmount);
        }
    }
}