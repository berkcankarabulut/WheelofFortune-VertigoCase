using _Project.Scripts.Data.Reward;
using _Project.Scripts.Service;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Config; 
using _Project.Scripts.Utils; 
using UnityEngine;

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelRewardSetter : MonoBehaviour
    {
        [Header("Reward UI References")] 
        [SerializeField] private WheelRewardUIElement[] _wheelRewardUIs;

        public void LoadRewards(int zone)
        {
            WheelDataSO wheelData = WheelDataService.Instance.GetConfigsForZone(zone);
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
                ItemAmountData itemAmountData = isBombSlice ? 
                    GetScaledReward(wheelData.GetBombRewardData(), currentZone) : 
                    GetScaledReward(wheelData.GetRandomRewardData(), currentZone);

                _wheelRewardUIs[i]?.SetRewardData(itemAmountData);
                
                if (isBombSlice)
                    this.Log($"Added BOMB to slice {i}");
                else
                    this.Log($"Set reward for slice {i}: {itemAmountData.ItemSo.Name} x{itemAmountData.Amount} (Zone {currentZone})");
            }
        }

        private ItemAmountData GetScaledReward(ItemAmountData baseItemAmount, int zone)
        {
            if (baseItemAmount == null) return null;
            
            float multiplier = Mathf.Pow(GameSettings.ZoneRewardMultiplier, zone - 1);
            if (zone % GameSettings.SuperZoneInterval == 0)
                multiplier *= GameSettings.SuperZoneMultiplier;

            int scaledAmount = Mathf.RoundToInt(baseItemAmount.Amount * multiplier);
            return new ItemAmountData(baseItemAmount.ItemSo, scaledAmount);
        }
    }
}