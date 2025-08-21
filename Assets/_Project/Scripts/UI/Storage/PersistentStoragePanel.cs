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

        protected override string GetDataId(RewardData data)
        {
            return data?.RewardItemSo?.Id.ToGuid().ToString() ?? string.Empty;
        }

        protected override IEnumerable<RewardData> GroupData(List<RewardData> dataList)
        {
            var grouped = dataList.GroupBy(r => r.RewardItemSo)
                .ToDictionary(g => g.Key.Id.ToGuid().ToString(), g => new RewardData(g.Key, g.Sum(r => r.Amount)));
            
            var result = new List<RewardData>();  
            foreach (var id in _orderedIds.ToList())
            {
                if (grouped.ContainsKey(id))
                {
                    result.Add(grouped[id]);
                    grouped.Remove(id);
                }
                else
                {
                    _orderedIds.Remove(id);
                }
            }  
            var newItems = grouped.Values.OrderBy(d => d.RewardItemSo.Name);
            foreach (var item in newItems)
            {
                result.Add(item);
                _orderedIds.Add(item.RewardItemSo.Id.ToGuid().ToString());
            }
            
            return result;
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