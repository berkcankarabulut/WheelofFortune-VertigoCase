using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Event.Save; 

namespace _Project.Scripts.Runtime.Storage
{
    public class PersistentItemStorage : MonoBehaviour
    {
        [SerializeField] private ItemDatabaseSO _itemDatabase;

        private List<RewardData> _items = new List<RewardData>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        private const string SAVE_KEY = "PlayerItems";

        public int Count => _items.Count;
        public System.Action OnLoaded { get; set; }

        private void Awake()
        { 
            MessageBroker.Default.Receive<OnSaveRequestedEvent>()
                .Subscribe(OnSaveRequested)
                .AddTo(_disposables);
        }

        private void Start()
        {
            LoadData();
        }

        private void OnSaveRequested(OnSaveRequestedEvent evt)
        {
            if (evt.CacheItems == null) return;
            foreach (var cacheItem in evt.CacheItems)
            {
                AddOrMerge(cacheItem);
            }

            SaveData();
        }

        public void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null) return;

            _items.Add(rewardData);
            PublishChangeEvent(StorageChangeType.Added, rewardData);
        }

        public bool Remove(RewardData item)
        {
            if (_items.Remove(item))
            {
                PublishChangeEvent(StorageChangeType.Removed, item);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            _items.Clear();
            PublishChangeEvent(StorageChangeType.Cleared);
        }

        public List<RewardData> GetAll()
        {
            return new List<RewardData>(_items);
        }

        public int GetTotalAmount(RewardItemSO rewardItem)
        {
            return _items.Where(item => ItemEquals(item, rewardItem)).Sum(item => item.Amount);
        }

        public List<RewardData> GetRewardsByType(RewardType type)
        {
            return _items.Where(item => item.RewardItemSo?.Type == type).ToList();
        }

        public bool RemoveReward(RewardItemSO rewardItem, int amount)
        {
            var matchingItems = _items.Where(item => ItemEquals(item, rewardItem)).ToList();
            int totalAvailable = matchingItems.Sum(item => item.Amount);

            if (totalAvailable < amount) return false;

            int remaining = amount;
            foreach (var item in matchingItems.OrderByDescending(i => i.Amount))
            {
                if (remaining <= 0) break;

                if (item.Amount <= remaining)
                {
                    remaining -= item.Amount;
                    _items.Remove(item);
                }
                else
                {
                    var newItem = new RewardData(item.RewardItemSo, item.Amount - remaining);
                    int index = _items.IndexOf(item);
                    _items[index] = newItem;
                    remaining = 0;
                }
            }

            PublishChangeEvent(StorageChangeType.Updated);
            return true;
        }

        private void AddOrMerge(RewardData newItem)
        {
            if (newItem?.RewardItemSo == null) return;

            var existingItem = _items.FirstOrDefault(item => ItemEquals(item, newItem.RewardItemSo));

            if (existingItem != null)
            { 
                var mergedItem = new RewardData(existingItem.RewardItemSo, existingItem.Amount + newItem.Amount);
                int index = _items.IndexOf(existingItem);
                _items[index] = mergedItem;
                PublishChangeEvent(StorageChangeType.Updated, mergedItem);
            }
            else
            { 
                _items.Add(newItem);
                PublishChangeEvent(StorageChangeType.Added, newItem);
            }
        }

        private bool ItemEquals(RewardData item, RewardItemSO rewardItem)
        {
            return item?.RewardItemSo != null &&
                   rewardItem != null &&
                   item.RewardItemSo.Id.Equals(rewardItem.Id);
        }

        private void SaveData()
        {
            try
            {
                var saveData = new ItemSaveData();

                foreach (var item in _items)
                {
                    if (item?.RewardItemSo != null)
                    {
                        saveData.items.Add(new ItemSaveData.SavedItem
                        {
                            itemId = item.RewardItemSo.Id.ToGuid().ToString(),
                            amount = item.Amount
                        });
                    }
                } 
                string json = JsonUtility.ToJson(saveData);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save(); 
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                if (!PlayerPrefs.HasKey(SAVE_KEY))
                {
                    OnLoadComplete();
                    return;
                }

                string json = PlayerPrefs.GetString(SAVE_KEY);
                var saveData = JsonUtility.FromJson<ItemSaveData>(json);

                if (saveData?.items == null)
                {
                    OnLoadComplete();
                    return;
                }

                _items.Clear();

                foreach (var savedItem in saveData.items)
                {
                    var itemSO = _itemDatabase?.GetItemById(savedItem.itemId);
                    if (itemSO != null)
                    {
                        _items.Add(new RewardData(itemSO, savedItem.amount));
                    }
                }

                Debug.Log($"Loaded {_items.Count} items from persistent storage");
                OnLoadComplete();
            }
            catch (Exception e)
            {
                Debug.LogError($"Load failed: {e.Message}");
                OnLoadComplete();
            }
        }

        private void OnLoadComplete()
        {
            OnLoaded?.Invoke();
            MessageBroker.Default.Publish(new OnSaveLoadedEvent());
            PublishChangeEvent(StorageChangeType.Loaded);
        }

        private void PublishChangeEvent(StorageChangeType changeType, RewardData changedItem = null)
        {
            var evt = new OnStorageChangedEvent<PersistentItemStorage, RewardData>(_items, changeType, changedItem);
            MessageBroker.Default.Publish(evt);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}