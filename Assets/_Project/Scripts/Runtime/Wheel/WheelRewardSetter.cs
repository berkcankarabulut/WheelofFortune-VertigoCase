using _Project.Scripts.Data.Reward;
using _Project.Scripts.Service;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Config;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Wheel
{
  public class WheelRewardSetter : MonoBehaviour, IWheelRewardSetter
    {
        [Header("Reward UI References")] 
        [SerializeField] private WheelRewardUIElement[] _wheelRewardUIs;
 
        private IWheelDataService _wheelDataService;
        private IGameSettings _gameSettings;

        [Inject]
        public void Construct(
            IWheelDataService wheelDataService,
            IGameSettings gameSettings)
        {
            _wheelDataService = wheelDataService;
            _gameSettings = gameSettings;
        }

        public void LoadRewards(int zone)
        {
            WheelDataSO wheelData = _wheelDataService.GetConfigsForZone(zone);
            SetRewardsForWheel(wheelData, zone);
        }

        private void SetRewardsForWheel(WheelDataSO wheelData, int currentZone)
        {
            if (!wheelData) return;  
            if (!wheelData.HasRewards || _wheelRewardUIs?.Length == 0) return;

            bool shouldAddBomb = wheelData.Type == WheelType.BronzeZone;
            int bombSliceIndex = shouldAddBomb ? Random.Range(0, _wheelRewardUIs.Length) : -1;
            
            for (int i = 0; i < _wheelRewardUIs.Length; i++)
            {
                bool isBombSlice = i == bombSliceIndex;
                RewardData rewardData = isBombSlice ? 
                    GetScaledReward(wheelData.GetBombRewardData(), currentZone) : 
                    GetScaledReward(wheelData.GetRandomRewardData(), currentZone);

                _wheelRewardUIs[i]?.SetRewardData(rewardData);
                
                if (isBombSlice)
                    this.Log($"Added BOMB to slice {i}");
                else
                    this.Log($"Set reward for slice {i}: {rewardData.RewardItemSo.Name} x{rewardData.Amount} (Zone {currentZone})");
            }
        }

        private RewardData GetScaledReward(RewardData baseReward, int zone)
        {
            if (baseReward == null) return null;
             
            float multiplier = Mathf.Pow(_gameSettings.ZoneRewardMultiplier, zone - 1);
            if (zone % _gameSettings.SuperZoneInterval == 0)
                multiplier *= _gameSettings.SuperZoneMultiplier;

            int scaledAmount = Mathf.RoundToInt(baseReward.Amount * multiplier);
            scaledAmount = Mathf.Clamp(scaledAmount, 0, baseReward.RewardItemSo.MaxLimit);
            return new RewardData(baseReward.RewardItemSo, scaledAmount);
        }
    }
}