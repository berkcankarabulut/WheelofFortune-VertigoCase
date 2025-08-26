using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool; 

namespace _Project.Scripts.UI.Storage
{
    public abstract class StoragePanel<TData, TUIElement> : MonoBehaviour
        where TUIElement : StorageUIElement<TData>
    {
        [SerializeField] protected Transform container;
        [SerializeField] protected TUIElement uiElementPrefab;
        [SerializeField] protected int poolDefaultCapacity = 5;
        [SerializeField] protected int poolMaxSize = 20;

        protected ObjectPool<TUIElement> _uiPool;
        protected List<TUIElement> _activeUIs = new List<TUIElement>();

        protected Dictionary<string, TUIElement> _uiMap = new Dictionary<string, TUIElement>();
        protected List<string> _orderedIds = new List<string>();

        protected virtual void Awake()
        {
            InitializePool();
        }

        protected virtual void InitializePool()
        {
            if (!uiElementPrefab)
                return;

            _uiPool = new ObjectPool<TUIElement>(
                CreatePooledItem,
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPooledItem,
                defaultCapacity: poolDefaultCapacity,
                maxSize: poolMaxSize
            );
        }

        protected virtual TUIElement CreatePooledItem()
        {
            var instance = Instantiate(uiElementPrefab, container);
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
            if (dataList?.Count > 0)
            {
                var groupedData = GroupData(dataList).ToList();
                var newMap = new Dictionary<string, TUIElement>();

                for (int i = 0; i < groupedData.Count; i++)
                {
                    var data = groupedData[i];
                    var id = GetDataId(data);

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

        public virtual void ClearDisplay()
        {
            foreach (var ui in _uiMap.Values) _uiPool?.Release(ui);
            _uiMap.Clear();
            _activeUIs.Clear();
            _orderedIds.Clear();
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

        protected virtual string GetDataId(TData data)
        {
            return data?.GetHashCode().ToString() ?? string.Empty;
        }

        protected virtual void OnDestroy()
        {
            ClearDisplay();
            _uiPool?.Dispose();
        }
    }
}