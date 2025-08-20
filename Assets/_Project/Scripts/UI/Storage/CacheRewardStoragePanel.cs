using System.Collections.Generic;
using System.Linq; 
using UniRx;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Storage; 

namespace _Project.Scripts.UI.Storage
{
    public class CacheRewardStoragePanel : StoragePanel<RewardData, CacheStorageUIElement>
    {
        private CompositeDisposable _disposables = new CompositeDisposable();

        protected override void Awake()
        {
            base.Awake();
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        { 
            MessageBroker.Default.Receive<OnCacheStorageChangedEvent>()
                .Subscribe(evt => DisplayRewards(evt.CacheData))
                .AddTo(_disposables);
        }

        protected override CacheStorageUIElement CreatePooledItem()
        {
            var instance = Instantiate(_uiElementPrefab, _container); 
            return instance;
        } 

        protected override IEnumerable<RewardData> GroupData(List<RewardData> dataList)
        {
            return dataList
                .GroupBy(r => r.RewardItemSo)
                .Select(g => new RewardData(g.Key, g.Sum(r => r.Amount)))
                .OrderBy(d => d.RewardItemSo.Name);
        }

        private void DisplayRewards(List<RewardData> rewardDataList)
        {
            DisplayData(rewardDataList);
        }

        protected override void OnDestroy()
        {
            _disposables?.Dispose();
            base.OnDestroy();
        }
    }
}