using System.Collections.Generic;
using System.Linq; 
using UniRx;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Reward;
using _Project.Scripts.Event.Storage;
using _Project.Scripts.Utils;
using AssetKits.ParticleImage;
using UnityEngine;

namespace _Project.Scripts.UI.Storage
{
    public class CacheRewardStoragePanel : StoragePanel<RewardData, CacheStorageUIElement>
    {
        [SerializeField] private ParticleImage _lootParticleImage;
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
             
            MessageBroker.Default.Receive<OnRewardCollectedEvent>()
                .Subscribe(_ => PlayLootParticleEffect())
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

        private void PlayLootParticleEffect()
        {
            if (_lootParticleImage == null || _activeUIs.Count == 0) return;
 
            var latestRewardUI = _activeUIs.LastOrDefault();
            if (latestRewardUI == null) return;
            AddressableAtlasLoader.LoadSprite(latestRewardUI.Data.RewardItemSo.Icon, _lootParticleImage);
            _lootParticleImage.attractorTarget = latestRewardUI.transform;
            _lootParticleImage.Play();
        }

        protected override void OnDestroy()
        {
            _disposables?.Dispose();
            base.OnDestroy();
        }
    }
}