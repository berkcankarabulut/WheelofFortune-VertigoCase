using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using _Project.Scripts.Data.Reward; 

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
                () => Instantiate(rewardStorageUIElementPrefab, _container),
                e => e.gameObject.SetActive(true),
                e => { e.ResetUI(); e.gameObject.SetActive(false); },
                e => { if (e) Destroy(e.gameObject); },
                defaultCapacity: 5,
                maxSize: 20
            );
        }

        public void DisplayRewards(List<ItemAmountData> rewardDataList)
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

        private void CreateStorageUIs(List<ItemAmountData> rewardDataList)
        {
            var groupedRewards = rewardDataList
                .GroupBy(r => r.ItemSo)
                .Select(g => new ItemAmountData(g.Key, g.Sum(r => r.Amount)))
                .OrderBy(d => d.ItemSo.Name);

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