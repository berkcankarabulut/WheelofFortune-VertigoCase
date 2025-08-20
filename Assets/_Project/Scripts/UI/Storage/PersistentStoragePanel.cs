using System.Collections.Generic;
using System.Linq; 
using UniRx;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Storage;
using UnityEngine;

namespace _Project.Scripts.UI.Storage
{
    public class PersistentStoragePanel : StoragePanel<RewardData, PersistentStorageUIElement>
    {
        [SerializeField] private GameObject _panelGO;
        private CompositeDisposable _disposables = new CompositeDisposable();

        protected override void Awake()
        {
            base.Awake(); 
            InitializeEventSubscriptions();  
        }

        private void InitializeEventSubscriptions()
        { 
            MessageBroker.Default.Receive<OnShowPersistentStorageRequested>()
                .Subscribe(_ => _panelGO.SetActive(!_panelGO.activeSelf))
                .AddTo(_disposables);
 
            MessageBroker.Default.Receive<OnPersistentStorageChangedEvent>()
                .Subscribe(evt => DisplayRewards(evt.PersistentData))
                .AddTo(_disposables);
        }

        protected override PersistentStorageUIElement CreatePooledItem()
        {
            var instance = Instantiate(_uiElementPrefab, _container);
            print("Loading persistent storage element");
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