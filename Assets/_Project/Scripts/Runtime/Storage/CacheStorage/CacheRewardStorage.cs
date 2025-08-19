using System.Collections.Generic;
using System.Linq; 
using _Project.Scripts.Data.Reward; 
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Utils;
using System;
using _Project.Scripts.Data.Item;

namespace _Project.Scripts.Runtime.Storage
{
    public class CacheRewardStorage : Storage<ItemAmountData>
    {  
        public event Action<ItemAmountData> OnRewardAdded;
        public event Action<ItemSO, int> OnRewardRemoved;
        public event Action OnStorageCleared;
        public event Action OnStorageChanged;

        protected override void InitializeStorage()
        {
            base.InitializeStorage();
            this.Log("InGameRewardStorage ready");
        } 

        public override void Add(ItemAmountData itemAmountData)
        {
            if (itemAmountData?.ItemSo == null)
            {
                this.LogWarning("Null reward data");
                return;
            } 

            base.Add(itemAmountData);
            this.Log($"Added: {itemAmountData.ItemSo.Name} x{itemAmountData.Amount}");
            
            // Trigger events
            OnRewardAdded?.Invoke(itemAmountData);
            OnStorageChanged?.Invoke();
        }

        public int GetTotalAmount(ItemSO item)
        {
            return _items
                .Where(reward => reward.ItemSo != null && reward.ItemSo.Equals(item))
                .Sum(reward => reward.Amount);
        }

        public List<ItemAmountData> GetRewardsByType(RewardType rewardType)
        {
            return _items
                .Where(reward => reward.ItemSo != null && reward.ItemSo.Type == rewardType)
                .ToList();
        }

        public List<ItemAmountData> GetRewardsByItem(ItemSO item)
        {
            return _items
                .Where(reward => reward.ItemSo != null && reward.ItemSo.Equals(item))
                .ToList();
        }

        public bool RemoveReward(ItemSO item, int amount)
        {
            var rewardsToRemove = GetRewardsByItem(item);
            int totalAmount = rewardsToRemove.Sum(r => r.Amount);

            if (totalAmount < amount)
            {
                this.LogWarning($"Not enough {item.Name}. Has: {totalAmount}, Needed: {amount}");
                return false;
            }

            int remainingToRemove = amount;
            for (int i = rewardsToRemove.Count - 1; i >= 0 && remainingToRemove > 0; i--)
            {
                var reward = rewardsToRemove[i];
                if (reward.Amount <= remainingToRemove)
                {
                    remainingToRemove -= reward.Amount;
                    _items.Remove(reward);
                }
                else
                {
                    // Partial removal - create new reward with reduced amount
                    var newReward = new ItemAmountData(reward.ItemSo, reward.Amount - remainingToRemove);
                    int index = _items.IndexOf(reward);
                    _items[index] = newReward;
                    remainingToRemove = 0;
                }
            }

            this.Log($"Removed: {item.Name} x{amount}");
            
            // Trigger events
            OnRewardRemoved?.Invoke(item, amount);
            OnStorageChanged?.Invoke();
            
            return true;
        }

        public override void Clear()
        {
            int clearedCount = Count;
            base.Clear();
            
            // Trigger events
            OnStorageCleared?.Invoke();
            OnStorageChanged?.Invoke();
        }

        public string GetSummary()
        {
            if (Count == 0)
                return "No rewards stored";

            var grouped = _items
                .Where(reward => reward.ItemSo != null)
                .GroupBy(r => r.ItemSo, new RewardItemSOComparer())
                .Select(g => new { Item = g.Key, Total = g.Sum(r => r.Amount) });

            string summary = "Reward Storage:\n";
            foreach (var group in grouped)
            {
                summary += $"- {group.Item.Name}: {group.Total}\n";
            }
            summary += $"Total Entries: {Count}";

            return summary;
        }
  
        protected virtual void OnDestroy()
        {
            // Event cleanup
            OnRewardAdded = null;
            OnRewardRemoved = null;
            OnStorageCleared = null;
            OnStorageChanged = null;
        }
    }
 
    public class RewardItemSOComparer : IEqualityComparer<ItemSO>
    {
        public bool Equals(ItemSO x, ItemSO y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(ItemSO obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
} 