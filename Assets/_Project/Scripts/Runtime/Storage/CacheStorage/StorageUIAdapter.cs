using System.Collections.Generic;
using _Project.Scripts.Data.Item;
using UnityEngine;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.UI.Storage;
using _Project.Scripts.Utils; 

namespace _Project.Scripts.UI.Controllers
{ 
    public class StorageUIAdapter : MonoBehaviour
    {  
        [Header("References")]
        [SerializeField] private CacheRewardStoragePanel _cacheRewardPanel;
        [SerializeField] private CacheRewardStorage _cacheRewardStorage; 

        private void Start()
        {  
            RefreshDisplay();
        }

        private void OnEnable()
        {
            if(_cacheRewardStorage == null) return; 
            _cacheRewardStorage.OnRewardAdded += OnCacheRewardAdded;
            _cacheRewardStorage.OnRewardRemoved += OnCacheRewardRemoved;
            _cacheRewardStorage.OnStorageCleared += OnStorageCleared;
            _cacheRewardStorage.OnStorageChanged += OnStorageChanged;
        }

        private void OnDisable()
        { 
            if(_cacheRewardStorage == null) return;
            _cacheRewardStorage.OnRewardAdded -= OnCacheRewardAdded;
            _cacheRewardStorage.OnRewardRemoved -= OnCacheRewardRemoved;
            _cacheRewardStorage.OnStorageCleared -= OnStorageCleared;
            _cacheRewardStorage.OnStorageChanged -= OnStorageChanged;
        }
 
        private void OnCacheRewardAdded(ItemAmountData itemAmountData)
        {
            this.Log($"Reward added: {itemAmountData.ItemSo.Name} - refreshing display");
            RefreshDisplay();
        }

        private void OnCacheRewardRemoved(ItemSO item, int amount)
        {
            this.Log($"Reward removed: {item.Name} x{amount} - refreshing display");
            RefreshDisplay();
        }

        private void OnStorageCleared()
        {
            this.Log("Storage cleared - refreshing display");
            RefreshDisplay();
        }

        private void OnStorageChanged()
        { 
            this.Log("Storage changed - refreshing display");
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (_cacheRewardPanel == null)
            {
                this.LogWarning("RewardStoragePanel not assigned");
                return;
            } 
            if (_cacheRewardStorage == null)
            {
                this.LogWarning("RewardStorage not assigned - clearing display");
                _cacheRewardPanel.ClearDisplay();
                return;
            } 
            
            List<ItemAmountData> currentRewards = _cacheRewardStorage?.GetAll();
            _cacheRewardPanel?.DisplayRewards(currentRewards);
 
        }  
    }
}