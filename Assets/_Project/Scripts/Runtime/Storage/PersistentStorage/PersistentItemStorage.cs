using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UniRx;
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Data.Reward; 
using _Project.Scripts.Event.Save;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Utils;
using Zenject;

namespace _Project.Scripts.Runtime.Storage
{
    public class PersistentItemStorage : Storage<RewardData>
    {
        [SerializeField] private string _saveFileName = "rewards.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        
        private ItemDatabaseSO _itemDatabase;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(ItemDatabaseSO itemDatabase) => _itemDatabase = itemDatabase;

        protected override void InitializeStorage()
        { 
            LoadFromFile();
            
            MessageBroker.Default.Receive<OnSaveRequestedEvent>()
                .Subscribe(OnSaveRequested)
                .AddTo(_disposables);
        }

        private void OnSaveRequested(OnSaveRequestedEvent saveEvent)
        {
            if (saveEvent.RewardsToSave == null) return;
            AddItemsWithMerge(saveEvent.RewardsToSave);
            SaveToFile();
            PublishChanges();
        }

        private void AddItemsWithMerge(List<RewardData> newItems)
        {
            foreach (var newItem in newItems)
            {
                if (newItem?.RewardItemSo == null) continue;

                var existingIndex = _items.FindIndex(item => 
                    item.RewardItemSo != null && 
                    item.RewardItemSo.Id.Equals(newItem.RewardItemSo.Id));

                if (existingIndex != -1)
                {
                    var existing = _items[existingIndex];
                    _items[existingIndex] = new RewardData(existing.RewardItemSo, existing.Amount + newItem.Amount);
                }
                else
                {
                    _items.Add(newItem);
                }
            }
        }

        private void SaveToFile()
        {
            try
            {
                var saveData = new ItemSaveData();
                foreach (var reward in _items)
                {
                    if (reward?.RewardItemSo != null)
                    {
                        saveData.items.Add(new ItemSaveData.SavedItem
                        {
                            itemId = reward.RewardItemSo.Id.ToGuid().ToString(),
                            amount = reward.Amount
                        });
                    }
                }

                File.WriteAllText(SavePath, JsonUtility.ToJson(saveData, true));
            }
            catch (Exception e)
            {
                this.LogError($"[PersistentStorage] Save failed: {e.Message}");
            }
        }

        private void LoadFromFile()
        {
            try
            {
                if (!File.Exists(SavePath)) return;

                var saveData = JsonUtility.FromJson<ItemSaveData>(File.ReadAllText(SavePath));
                if (saveData?.items != null)
                {
                    _items.Clear();
                    foreach (var savedItem in saveData.items)
                    {
                        var rewardItem = _itemDatabase.GetItemById(savedItem.itemId);
                        if (rewardItem != null)
                            _items.Add(new RewardData(rewardItem, savedItem.amount));
                    }
                }

                MessageBroker.Default.Publish(new OnSaveLoadedEvent());
                PublishChanges();
            }
            catch (Exception e)
            {
                Debug.LogError($"[PersistentStorage] Load failed: {e.Message}");
            }
        }

        private void PublishChanges()
        {
            MessageBroker.Default.Publish(new OnPersistentStorageChangedEvent(GetAll()));
        }

        public override void Add(RewardData item)
        {
            base.Add(item);
            SaveToFile();
            PublishChanges();
        }

        public override bool Remove(RewardData item)
        {
            bool removed = base.Remove(item);
            if (removed)
            {
                SaveToFile();
                PublishChanges();
            }
            return removed;
        }

        public override void Clear()
        {
            base.Clear();
            SaveToFile();
            PublishChanges();
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}