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
    }
}