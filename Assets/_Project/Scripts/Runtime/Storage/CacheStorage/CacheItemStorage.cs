using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Interfaces;

namespace _Project.Scripts.Runtime.Storage
{
    public class CacheItemStorage : Storage<RewardData>, IItemStorage
    {
        protected override void InitializeStorage()
        {
            // Cache storage initialization if needed
        }

        protected override void PublishStorageEvent(List<RewardData> items, StorageChangeType changeType, RewardData changedItem)
        {
            Debug.Log($"CacheItemStorage: Publishing event with {items.Count} items, change type: {changeType}");
            
            var evt = new OnStorageChangedEvent<CacheItemStorage, RewardData>(items, changeType, changedItem);
            MessageBroker.Default.Publish(evt);
            
            Debug.Log("CacheItemStorage: Event published successfully");
        }

        public override void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null) return;
            
            Debug.Log($"Adding reward: {rewardData.RewardItemSo.Name} x{rewardData.Amount}");
            base.Add(rewardData);
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
            if (rewards.Sum(r => r.Amount) < amount) return false;

            int remaining = amount;
            for (int i = rewards.Count - 1; i >= 0 && remaining > 0; i--)
            {
                var reward = rewards[i];
                if (reward.Amount <= remaining)
                {
                    remaining -= reward.Amount;
                    Remove(reward);
                }
                else
                {
                    var newReward = new RewardData(reward.RewardItemSo, reward.Amount - remaining);
                    int index = _items.IndexOf(reward);
                    _items[index] = newReward;
                    remaining = 0;
                    PublishStorageEvent(StorageChangeType.Updated, newReward);
                }
            }
            
            return true;
        }
    }
}