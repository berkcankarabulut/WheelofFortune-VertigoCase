using System;
using System.IO;
using UnityEngine;
using UniRx;
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Data.Reward; 
using _Project.Scripts.Event.Save;
using _Project.Scripts.Event.Storage;
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
        public void Construct(ItemDatabaseSO itemDatabase)
        {
            Debug.Log("_itemDatabase");
            _itemDatabase = itemDatabase;
        }

        protected override void InitializeStorage()
        {
            base.InitializeStorage();
            LoadRewards();

            MessageBroker.Default.Receive<OnSaveRequestedEvent>()
                .Subscribe(OnSaveRequested)
                .AddTo(_disposables);
        }

        private void OnSaveRequested(OnSaveRequestedEvent saveEvent)
        {
            if (saveEvent.RewardsToSave != null)
            {
                _items.Clear();
                _items.AddRange(saveEvent.RewardsToSave);
            }
            SaveRewards();
            PublishStorageChanged();
        }

        private void SaveRewards()
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

                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, json);
                
                Debug.Log($"[PersistentStorage] Saved {saveData.items.Count} items");
            }
            catch (Exception e)
            {
                Debug.LogError($"[PersistentStorage] Save error: {e.Message}");
            }
        }

        private void LoadRewards()
        {
            try
            {
                if (!File.Exists(SavePath)) return;
                
                string json = File.ReadAllText(SavePath);
                var saveData = JsonUtility.FromJson<ItemSaveData>(json);

                if (saveData?.items != null)
                {
                    _items.Clear();
                    
                    foreach (var savedItem in saveData.items)
                    {
                        var rewardItem = _itemDatabase.GetItemById(savedItem.itemId);
                        if (rewardItem != null)
                        {
                            _items.Add(new RewardData(rewardItem, savedItem.amount));
                        }
                    }
                }
                
                MessageBroker.Default.Publish(new OnSaveLoadedEvent());
                PublishStorageChanged();
            }
            catch (Exception e)
            {
                Debug.LogError($"[PersistentStorage] Load error: {e.Message}");
            }
        }

        private void PublishStorageChanged()
        {
            MessageBroker.Default.Publish(new OnPersistentStorageChangedEvent(GetAll()));
        }

        public override void Add(RewardData item)
        {
            base.Add(item);
            SaveRewards();
            PublishStorageChanged();
        }

        public override bool Remove(RewardData item)
        {
            bool removed = base.Remove(item);
            if (removed)
            {
                SaveRewards();
                PublishStorageChanged();
            }
            return removed;
        }

        public override void Clear()
        {
            base.Clear();
            SaveRewards();
            PublishStorageChanged();
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}