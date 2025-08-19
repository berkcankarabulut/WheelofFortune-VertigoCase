using _Project.Scripts.Data.Reward;
using _Project.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Storage
{
    public class RewardStorageUIElement : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image _rewardIcon_value;
        [SerializeField] private TextMeshProUGUI _rewardAmount_value;  

        private ItemAmountData _data;

        public void SetData(ItemAmountData data)
        {
            _data = data;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_data?.ItemSo == null)
                return;

            if (_data.ItemSo.Type == RewardType.Bomb)
                _rewardAmount_value.text = "Bomb";
            else if (_rewardAmount_value != null)
                _rewardAmount_value.text =  NumberFormatter.FormatDecimal(_data.Amount);
 
            if (_rewardIcon_value != null)
                LoadRewardIcon();
        }

        private void LoadRewardIcon()
        {
            if (_data.ItemSo.Icon != null)
            {
                AdressableAtlasManager.LoadSprite(_data.ItemSo.Icon, _rewardIcon_value);
            }
        }

        // Reset method for pool reuse
        public void ResetUI()
        {
            _data = null;
            if (_rewardIcon_value != null)
                _rewardIcon_value.sprite = null;
            if (_rewardAmount_value != null)
                _rewardAmount_value.text = "";
        }
    }
}