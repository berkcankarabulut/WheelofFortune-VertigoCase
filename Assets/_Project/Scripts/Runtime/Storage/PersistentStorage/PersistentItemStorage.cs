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
    public class PersistentItemStorage : Storage<ItemAmountData>
    {
        [SerializeField] private string _saveFileName = "rewards.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        private CompositeDisposable _disposables = new CompositeDisposable();
        public Action OnLoaded;
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
            }
            catch (Exception e)
            {
                this.LogError($"❌ Save failed: {e.Message}");
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
                    this.Log($"✅ Loaded {Count} rewards");
                }
                OnLoaded.Invoke();
            }
            catch (Exception e)
            {
                this.LogError($"❌ Load failed: {e.Message}");
            }
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}