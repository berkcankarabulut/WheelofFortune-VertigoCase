using _Project.Scripts.Data.Reward; 
using _Project.Scripts.Utils; 
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

namespace _Project.Scripts.UI.Wheel
{
    public class WheelRewardUIElement : MonoBehaviour
    {
        [SerializeField] private Image _rewardImage;
        [SerializeField] private TextMeshProUGUI _amountText_value;  
        private RewardData _rewardData; 

        public void SetRewardData(RewardData rewardData)
        {
            _rewardData = rewardData;

            if (_rewardData?.RewardItemSo != null)
            {
                _amountText_value.text = rewardData.RewardItemSo.Type == RewardType.Bomb
                    ? "Bomb"
                    : "x"+NumberFormatter.FormatDecimal(_rewardData.Amount);

                AddressableAtlasLoader.LoadSprite(_rewardData.RewardItemSo.Icon, _rewardImage);
            }
            else
            {
                _amountText_value.text = "";
                _rewardImage.sprite = null;
            }
        }

        public RewardData GetRewardData()
        {
            return _rewardData;
        }
    }
}