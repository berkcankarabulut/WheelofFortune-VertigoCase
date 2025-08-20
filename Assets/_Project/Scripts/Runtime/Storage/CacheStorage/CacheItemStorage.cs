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
        public event Action<RewardData> OnItemAdded;
        public event Action<RewardItemSO, int> OnItemRemoved;
        public event Action OnStorageCleared;
        public event Action OnStorageChanged;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private void Awake()
        { 
            MessageBroker.Default.Receive<OnGameOveredEvent>()
                .Subscribe(OnGiveUp)
                .AddTo(_disposables);
        }

        private void OnGiveUp(OnGameOveredEvent e)
        {
            OnStorageCleared?.Invoke();
        }
        
        protected override void InitializeStorage()
        {
            base.InitializeStorage(); 
        }

        public override void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null)
            { 
                return;
            }   
            base.Add(rewardData); 
            
            OnItemAdded?.Invoke(rewardData);
            OnStorageChanged?.Invoke();
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
 
            OnItemRemoved?.Invoke(rewardItem, amount);
            OnStorageChanged?.Invoke();
            PublishStorageChanged();
            return true;
        }

        public override void Clear()
        {
            base.Clear();
            OnStorageCleared?.Invoke();
            OnStorageChanged?.Invoke();
            PublishStorageChanged();
        }

        private void PublishStorageChanged()
        {
            var cacheStorageEvent = new OnCacheStorageChangedEvent(GetAll());
            MessageBroker.Default.Publish(cacheStorageEvent);
        }

        protected virtual void OnDestroy()
        {
            OnItemAdded = null;
            OnItemRemoved = null;
            OnStorageCleared = null;
            OnStorageChanged = null;
        } 
    }
}