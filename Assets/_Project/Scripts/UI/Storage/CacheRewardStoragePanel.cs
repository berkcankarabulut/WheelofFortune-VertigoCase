using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using _Project.Scripts.Data.Reward; 
using Zenject;

namespace _Project.Scripts.UI.Storage
{
    public class CacheRewardStoragePanel : MonoBehaviour
    {
        [SerializeField] private Transform _container; 
        [SerializeField] private RewardStorageUIElement rewardStorageUIElementPrefab;

        private ObjectPool<RewardStorageUIElement> _storageUIPool;
        private List<RewardStorageUIElement> _activeStorageUIs = new List<RewardStorageUIElement>();  

        private void Start()
        {
            if (!rewardStorageUIElementPrefab)
            { 
                return;
            }

            _storageUIPool = new ObjectPool<RewardStorageUIElement>(
                CreatePooledItem,
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPooledItem,
                defaultCapacity: 5,
                maxSize: 20
            );
        }

        private RewardStorageUIElement CreatePooledItem()
        {
            var instance = Instantiate(rewardStorageUIElementPrefab, _container); 
            print("Loading reward storage element");
            return instance;
        }

        private void OnGetFromPool(RewardStorageUIElement element)
        {
            element.gameObject.SetActive(true); 
        }

        private void OnReturnToPool(RewardStorageUIElement element)
        {
            element.ResetUI();
            element.gameObject.SetActive(false);
        }

        private void OnDestroyPooledItem(RewardStorageUIElement element)
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