using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx; 
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.StorageSystem;
using Zenject;

namespace _Project.Scripts.Runtime.Storage
{
    public class PersistentItemStorage : Storage<RewardData>, IItemStorage
    {
        [Inject] private ItemDatabaseSO _itemDatabase;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        private const string SAVE_KEY = "PlayerItems"; 

        protected override void InitializeStorage()
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

        public override void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null) return;
            
            base.Add(rewardData);
            SaveData();
        }

        public override bool Remove(RewardData item)
        {
            bool removed = base.Remove(item);
            if (removed) SaveData();
            PublishStorageEvent(StorageChangeType.Updated, item);
            return removed; 
        }

        public override void Clear()
        {
            base.Clear();
            SaveData();
        } 
        
        private void AddOrMerge(RewardData newItem)
        {
            if (newItem?.RewardItemSo == null) return;

            var existingItem = items.FirstOrDefault(item => ItemEquals(item, newItem.RewardItemSo));

            if (existingItem != null)
            { 
                var mergedItem = new RewardData(existingItem.RewardItemSo, existingItem.Amount + newItem.Amount);
                int index = items.IndexOf(existingItem);
                items[index] = mergedItem;
                PublishStorageEvent(StorageChangeType.Updated, mergedItem);
            }
            else
            { 
                items.Add(newItem);
                PublishStorageEvent(StorageChangeType.Added, newItem);
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

                foreach (var item in items)
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
                Debug.LogError($"PersistentStorage: Save failed - {e.Message}");
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

                items.Clear();

                foreach (var savedItem in saveData.items)
                {
                    var itemSO = _itemDatabase?.GetItemById(savedItem.itemId);
                    if (itemSO != null)
                    {
                        items.Add(new RewardData(itemSO, savedItem.amount));
                    }
                }

                OnLoadComplete();
            }
            catch (Exception e)
            {
                Debug.LogError($"PersistentStorage: Load failed - {e.Message}");
                OnLoadComplete();
            }
        }

        private void OnLoadComplete()
        { 
            MessageBroker.Default.Publish(new OnSaveLoadedEvent());
            PublishStorageEvent(StorageChangeType.Loaded);
        }

        protected override void PublishStorageEvent(List<RewardData> items, StorageChangeType changeType, RewardData changedItem)
        {  
            var evt = new OnStorageChangedEvent<PersistentItemStorage, RewardData>(items, changeType, changedItem);
            MessageBroker.Default.Publish(evt); 
        }
        
        protected virtual void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}