using _Project.Scripts.Data.Reward; 
using _Project.Scripts.Utils; 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Storage
{
    public class PersistentStorageUIElement : StorageUIElement<RewardData>
    {
        [Header("UI Components")]
        [SerializeField] private Image _rewardIcon_value;
        [SerializeField] private TextMeshProUGUI _rewardAmount_value;

        protected override void UpdateDisplay()
        {
            if (_data?.RewardItemSo == null)
                return;

            if (_data.RewardItemSo.Type == RewardType.Bomb)
                _rewardAmount_value.text = "Bomb";
            else if (_rewardAmount_value != null)
                _rewardAmount_value.text = NumberFormatter.FormatDecimal(_data.Amount);
 
            if (_rewardIcon_value != null)
                LoadRewardIcon();
        }

        private void LoadRewardIcon()
        { 
            if (_data.RewardItemSo.Icon == null) return;
            
            AddressableAtlasLoader.LoadSprite(_data.RewardItemSo.Icon,
                sprite => { _rewardIcon_value.sprite = sprite; }); 
        }

        protected override void ClearDisplay()
        {
            if (_rewardIcon_value != null)
                _rewardIcon_value.sprite = null;
            if (_rewardAmount_value != null)
                _rewardAmount_value.text = "";
        }
    }
}