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

        private ItemAmountData _itemAmountData;

        public void SetRewardData(ItemAmountData itemAmountData)
        {
            _itemAmountData = itemAmountData;

            if (_itemAmountData?.ItemSo != null)
            {
                _amountText_value.text = itemAmountData.ItemSo.Type == RewardType.Bomb
                    ? "Bomb"
                    : NumberFormatter.FormatDecimal(_itemAmountData.Amount);
                AdressableAtlasManager.LoadSprite(_itemAmountData.ItemSo.Icon, _rewardImage);
            }
            else
            {
                _amountText_value.text = "";
                _rewardImage.sprite = null;
            }
        }

        public ItemAmountData GetRewardData()
        {
            return _itemAmountData;
        }
    }
}