using System.Collections.Generic;
using System.Linq; 
using UniRx;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Storage;
using AssetKits.ParticleImage;
using UnityEngine;

namespace _Project.Scripts.UI.Storage
{
    public class CacheRewardStoragePanel : StoragePanel<RewardData, CacheStorageUIElement>
    {
        [SerializeField] private ParticleImage _lootParticleImage;
        private CompositeDisposable _disposables = new CompositeDisposable(); 
        private Dictionary<string, CacheStorageUIElement> _uiMap = new Dictionary<string, CacheStorageUIElement>();
        private List<string> _orderedIds = new List<string>();

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

        public override void DisplayData(List<RewardData> dataList)
        {
            if (dataList?.Count > 0)
            {
                var groupedData = GroupData(dataList).ToList();
                var newMap = new Dictionary<string, CacheStorageUIElement>();
                
                for (int i = 0; i < groupedData.Count; i++)
                {
                    var data = groupedData[i];
                    var id = data.RewardItemSo.Id.ToGuid().ToString(); 
                    var ui = _uiMap.ContainsKey(id) ? _uiMap[id] : _uiPool.Get();
                    if (_uiMap.ContainsKey(id)) _uiMap.Remove(id);
                    
                    ui.SetData(data);
                    ui.transform.SetSiblingIndex(i);
                    newMap[id] = ui;
                } 
                foreach (var ui in _uiMap.Values) _uiPool.Release(ui);
                
                _uiMap = newMap;
                _activeUIs = _uiMap.Values.ToList();
            }
            else
            {
                ClearDisplay();
            }
        }

        public override void ClearDisplay()
        {
            foreach (var ui in _uiMap.Values) _uiPool?.Release(ui);
            _uiMap.Clear();
            _activeUIs.Clear();
            _orderedIds.Clear();
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