using System;
using System.IO;
using UnityEngine;
using UniRx;
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Utils;

namespace _Project.Scripts.Runtime.Storage
{
    public class PersistentItemStorage : Storage<RewardData>
    {
        [SerializeField] private string _saveFileName = "rewards.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        private CompositeDisposable _disposables = new CompositeDisposable();
 
        protected override void InitializeStorage()
        {
            base.InitializeStorage();
            LoadRewards();

            MessageBroker.Default.Receive<OnSaveRequestedEvent>()
                .Subscribe(OnSaveRequested)
                .AddTo(_disposables);

            this.Log("PersistentItemStorage initialized");
        }

        private void OnSaveRequested(OnSaveRequestedEvent saveEvent)
        {
            if (saveEvent.RewardsToSave != null)
            {
                Clear();
                _items.AddRange(saveEvent.RewardsToSave);
            }

            SaveRewards();
        } 
        private void SaveRewards()
        {
            try
            {
                var saveData = new ItemSaveData { rewards = GetAll() };
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SavePath, json);
                this.Log($"✅ Saved {Count} rewards");
                for (int i = 0; i < _items.Count; i++)
                {
                    print(_items[i].RewardItemSo +" // " +_items[i].Amount);
                }
            }
            catch (Exception e)
            {
                this.LogError($"Save failed: {e.Message}");
            }
        }

        private void LoadRewards()
        {
            try
            {
                if (!File.Exists(SavePath)) return;
                string json = File.ReadAllText(SavePath);
                var saveData = JsonUtility.FromJson<ItemSaveData>(json);
                if (saveData?.rewards != null)
                {
                    Clear();
                    _items.AddRange(saveData.rewards);
                    this.Log($"Loaded {Count} rewards");
                }
                
                for (int i = 0; i < _items.Count; i++)
                {
                    print(_items[i].RewardItemSo +" // " +_items[i].Amount);
                }

                Debug.LogWarning("Yüklendi");
                MessageBroker.Default.Publish(new OnSaveLoadedEvent());
            }
            catch (Exception e)
            {
                this.LogError($"Load failed: {e.Message}");
            }
        }

        public void Remove(RewardData reward)
        {
            bool removeResult = base.Remove(reward);
            if (removeResult)
                SaveRewards();
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}