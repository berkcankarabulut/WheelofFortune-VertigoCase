using System.Collections.Generic;
using _Project.Scripts.Data.Item;
using UnityEngine;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.UI.Storage; 
using Zenject;

namespace _Project.Scripts.UI.Controllers
{
    public class StorageUIAdapter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CacheRewardStoragePanel _cacheRewardPanel; 
        private CacheItemStorage _cacheItemStorage;

        [Inject]
        public void Construct(CacheItemStorage cacheItemStorage)
        {
            _cacheItemStorage = cacheItemStorage;
        }

        private void Start()
        {
            RefreshDisplay();
        }

        private void OnEnable()
        {
            if (_cacheItemStorage == null) return;
            if (_cacheItemStorage is not CacheItemStorage concreteStorage) return;
            concreteStorage.OnItemAdded += OnCacheItemAdded;
            concreteStorage.OnItemRemoved += OnCacheItemRemoved;
            concreteStorage.OnStorageCleared += OnStorageCleared;
            concreteStorage.OnStorageChanged += OnStorageChanged;
        }

        private void OnDisable()
        {
            if (_cacheItemStorage == null) return;
            if (_cacheItemStorage is not CacheItemStorage concreteStorage) return;
            concreteStorage.OnItemAdded -= OnCacheItemAdded;
            concreteStorage.OnItemRemoved -= OnCacheItemRemoved;
            concreteStorage.OnStorageCleared -= OnStorageCleared;
            concreteStorage.OnStorageChanged -= OnStorageChanged;
        }

        private void OnCacheItemAdded(RewardData rewardData)
        { 
            RefreshDisplay();
        }

        private void OnCacheItemRemoved(RewardItemSO rewardItem, int amount)
        { 
            RefreshDisplay();
        }

        private void OnStorageCleared()
        { 
            RefreshDisplay();
        }

        private void OnStorageChanged()
        { 
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (_cacheRewardPanel == null)
            { 
                return;
            }

            if (_cacheItemStorage == null)
            { 
                _cacheRewardPanel.ClearDisplay();
                return;
            }

            List<RewardData> currentRewards = _cacheItemStorage?.GetAll();
            _cacheRewardPanel?.DisplayRewards(currentRewards);
        }
    }
}