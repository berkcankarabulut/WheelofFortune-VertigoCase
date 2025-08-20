using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.UI.Storage
{
    public abstract class StoragePanel<TData, TUIElement> : MonoBehaviour 
        where TUIElement : StorageUIElement<TData>
    {
        [SerializeField] protected Transform _container;
        [SerializeField] protected TUIElement _uiElementPrefab;

        protected ObjectPool<TUIElement> _uiPool;
        protected List<TUIElement> _activeUIs = new List<TUIElement>();

        protected virtual void Awake()
        {
            InitializePool();
        }

        protected virtual void InitializePool()
        {
            if (!_uiElementPrefab)
                return;

            _uiPool = new ObjectPool<TUIElement>(
                CreatePooledItem,
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPooledItem,
                defaultCapacity: 5,
                maxSize: 20
            );
        }

        protected virtual TUIElement CreatePooledItem()
        {
            var instance = Instantiate(_uiElementPrefab, _container);
            return instance;
        }

        protected virtual void OnGetFromPool(TUIElement element)
        {
            element.gameObject.SetActive(true);
        }

        protected virtual void OnReturnToPool(TUIElement element)
        {
            element.ResetUI();
            element.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyPooledItem(TUIElement element)
        {
            if (element) Destroy(element.gameObject);
        }

        public virtual void DisplayData(List<TData> dataList)
        {
            ClearDisplay();
            
            if (dataList?.Count > 0)
                CreateUIs(dataList);
        }

        public virtual void ClearDisplay()
        {
            _activeUIs.ForEach(ui => _uiPool?.Release(ui));
            _activeUIs.Clear();
        }

        protected virtual void CreateUIs(List<TData> dataList)
        {
            var groupedData = GroupData(dataList);

            foreach (var data in groupedData)
            {
                var ui = _uiPool.Get();
                ui.SetData(data);
                _activeUIs.Add(ui);
            }
        }

        protected abstract IEnumerable<TData> GroupData(List<TData> dataList);

        protected virtual void OnDestroy()
        {
            ClearDisplay();
            _uiPool?.Dispose();
        }
    }
}