using System.Collections.Generic;
using System.Linq; 
using UniRx; 
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Reward;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Interfaces;

namespace _Project.Scripts.Runtime.StorageSystem
{
    public class CacheItemStorage : Storage<RewardData>, IItemStorage
    {
        private CompositeDisposable _disposables = new CompositeDisposable();
        protected override void InitializeStorage()
        {   
            MessageBroker.Default.Receive<OnTryCollectRewardEvent>()
                .Subscribe(OnTryCollectItem)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnSafeExitRequestedEvent>()
                .Subscribe(OnSafeExitGame)
                .AddTo(_disposables);
        }
        
        private void OnTryCollectItem(OnTryCollectRewardEvent tryCollectRewardEvent)
        {
            RewardData rewardData = tryCollectRewardEvent.RewardData;
            if (rewardData?.RewardItemSo == null) return;

            if (rewardData.RewardItemSo.Type == RewardType.Bomb)
            {
                Clear();
            }
            else
            {
                Add(rewardData);
                MessageBroker.Default.Publish(new OnRewardCollectedEvent());
            }
        }

        private void OnSafeExitGame(OnSafeExitRequestedEvent safeExitEvent)
        {
            if (!safeExitEvent.ConfirmExit) return;
            List<RewardData> currentRewards = GetAll();
            OnSaveRequestedEvent saveEvent = new OnSaveRequestedEvent(currentRewards);

            MessageBroker.Default.Publish(saveEvent);
        }
        
        protected override void PublishStorageEvent(List<RewardData> items, StorageChangeType changeType, RewardData changedItem)
        {  
            var evt = new OnStorageChangedEvent<CacheItemStorage, RewardData>(items, changeType, changedItem);
            MessageBroker.Default.Publish(evt); 
        }

        public override void Add(RewardData rewardData)
        {
            if (rewardData?.RewardItemSo == null) return;
             
            base.Add(rewardData);
        }

        public int GetTotalAmount(RewardItemSO rewardItem) =>
            items.Where(i => i.RewardItemSo?.Equals(rewardItem) == true).Sum(i => i.Amount);

        public List<RewardData> GetRewardsByType(RewardType type) =>
            items.Where(i => i.RewardItemSo?.Type == type).ToList();

        public List<RewardData> GetRewardsByItem(RewardItemSO rewardItem) =>
            items.Where(i => i.RewardItemSo?.Equals(rewardItem) == true).ToList();

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
                    int index = items.IndexOf(reward);
                    items[index] = newReward;
                    remaining = 0;
                    PublishStorageEvent(StorageChangeType.Updated, newReward);
                }
            }
            
            return true;
        }
    }
}