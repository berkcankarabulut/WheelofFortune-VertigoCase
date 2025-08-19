using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using _Project.Scripts.Data.Reward;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.UI.Storage
{
    public class CacheRewardStoragePanel : MonoBehaviour
    {
        [SerializeField] private Transform _container; 
        [SerializeField] private CacheStorageUIElement cacheStorageUIElementPrefab;

        private ObjectPool<CacheStorageUIElement> _storageUIPool;
        private List<CacheStorageUIElement> _activeStorageUIs = new List<CacheStorageUIElement>();  

        private void Start()
        {
            if (!cacheStorageUIElementPrefab)
            { 
                return;
            }

            _storageUIPool = new ObjectPool<CacheStorageUIElement>(
                CreatePooledItem,
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPooledItem,
                defaultCapacity: 5,
                maxSize: 20
            );
        }

        private CacheStorageUIElement CreatePooledItem()
        {
            var instance = Instantiate(cacheStorageUIElementPrefab, _container); 
            print("Loading reward storage element");
            return instance;
        }

        private void OnGetFromPool(CacheStorageUIElement element)
        {
            element.gameObject.SetActive(true); 
        }

        private void OnReturnToPool(CacheStorageUIElement element)
        {
            element.ResetUI();
            element.gameObject.SetActive(false);
        }

        private void OnDestroyPooledItem(CacheStorageUIElement element)
        {
            if (element) Destroy(element.gameObject);
        }

        public void DisplayRewards(List<RewardData> rewardDataList)
        {
            ClearDisplay();
            
            if (rewardDataList?.Count > 0)
                CreateStorageUIs(rewardDataList);
        }

        public void ClearDisplay()
        {
            _activeStorageUIs.ForEach(ui => _storageUIPool?.Release(ui));
            _activeStorageUIs.Clear();
        }

        private void CreateStorageUIs(List<RewardData> rewardDataList)
        {
            var groupedRewards = rewardDataList
                .GroupBy(r => r.RewardItemSo)
                .Select(g => new RewardData(g.Key, g.Sum(r => r.Amount)))
                .OrderBy(d => d.RewardItemSo.Name);

            foreach (var data in groupedRewards)
            {
                var ui = _storageUIPool.Get();
                ui.SetData(data);
                _activeStorageUIs.Add(ui);
            }
        }

        private void OnDestroy()
        {
            ClearDisplay();
            _storageUIPool?.Dispose();
        }
    }
}