using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Runtime.Storage
{
    public class CacheItemStorage : Storage<RewardData>, IItemStorage
    {
        public event Action<RewardData> OnItemAdded;
        public event Action<RewardItemSO, int> OnItemRemoved;
        public event Action OnStorageCleared;
        public event Action OnStorageChanged;

        protected override void InitializeStorage()
        {
            base.InitializeStorage();
            this.Log("CacheItemStorage ready");
        }

        public override void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null)
            {
                this.LogWarning("Null item data");
                return;
            } 
            base.Add(rewardData);
            this.Log($"Added: {rewardData.RewardItemSo.Name} x{rewardData.Amount}");
            
            OnItemAdded?.Invoke(rewardData);
            OnStorageChanged?.Invoke();
        }

        public int GetTotalAmount(RewardItemSO rewardItem) =>
            _items.Where(i => i.RewardItemSo?.Equals(rewardItem) == true).Sum(i => i.Amount);

        public List<RewardData> GetRewardsByType(RewardType type) =>
            _items.Where(i => i.RewardItemSo?.Type == type).ToList();

        public List<RewardData> GetRewardsByItem(RewardItemSO rewardItem) =>
            _items.Where(i => i.RewardItemSo?.Equals(rewardItem) == true).ToList();

        public bool RemoveReward(RewardItemSO rewardItem, int amount)
        {
            var rewards = GetRewardsByItem(rewardItem);
            if (rewards.Sum(r => r.Amount) < amount)
            {
                this.LogWarning($"Insufficient {rewardItem.Name}. Has: {GetTotalAmount(rewardItem)}, Needed: {amount}");
                return false;
            }

            int remaining = amount;
            for (int i = rewards.Count - 1; i >= 0 && remaining > 0; i--)
            {
                var reward = rewards[i];
                if (reward.Amount <= remaining)
                {
                    remaining -= reward.Amount;
                    _items.Remove(reward);
                }
                else
                {
                    var newReward = new RewardData(reward.RewardItemSo, reward.Amount - remaining);
                    _items[_items.IndexOf(reward)] = newReward;
                    remaining = 0;
                }
            }

            this.Log($"Removed: {rewardItem.Name} x{amount}");
            OnItemRemoved?.Invoke(rewardItem, amount);
            OnStorageChanged?.Invoke();
            return true;
        }

        public override void Clear()
        {
            base.Clear();
            OnStorageCleared?.Invoke();
            OnStorageChanged?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            OnItemAdded = null;
            OnItemRemoved = null;
            OnStorageCleared = null;
            OnStorageChanged = null;
        }

#if UNITY_EDITOR
        [ContextMenu("📊 Debug Cache Storage")]
        private void DebugCacheStorage()
        {
            this.Log($"=== CACHE STORAGE DEBUG ===");
            this.Log($"Total items in cache: {Count}");
            this.Log($"Items list count: {_items?.Count ?? 0}");
            
            if (_items != null)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    this.Log($"[{i}] {item.RewardItemSo?.Name} x{item.Amount}");
                }
            }
            
            this.Log($"Events subscribed:");
            this.Log($"  OnItemAdded: {(OnItemAdded != null ? "YES" : "NO")}");
            this.Log($"  OnStorageChanged: {(OnStorageChanged != null ? "YES" : "NO")}");
        }

        [ContextMenu("🧪 Test Add Item")]
        private void TestAddItem()
        {
            // Test için basit bir RewardData oluştur (sadece test amaçlı)
            this.Log("Testing add functionality...");
        }
#endif
    }
}