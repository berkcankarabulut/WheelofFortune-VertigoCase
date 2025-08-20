using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Core.Storage;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Interfaces; 
using UniRx;

namespace _Project.Scripts.Runtime.Storage
{
    public class CacheItemStorage : Storage<RewardData>, IItemStorage
    {
        protected override void InitializeStorage()
        {
             
        }

        public override void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null)
            { 
                return;
            }   
            base.Add(rewardData); 
              
            PublishStorageChanged();
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
            if (rewards.Sum(r => r.Amount) < amount)  return false;

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
            PublishStorageChanged();
            return true;
        }

        public override void Clear()
        {
            base.Clear(); 
            PublishStorageChanged();
        }

        private void PublishStorageChanged()
        {
            var cacheStorageEvent = new OnCacheStorageChangedEvent(GetAll());
            MessageBroker.Default.Publish(cacheStorageEvent);
        } 
    }
}