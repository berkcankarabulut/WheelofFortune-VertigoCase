using System.Collections.Generic; 
using UnityEngine;

namespace _Project.Scripts.Core.Storage
{
    public abstract class Storage<T> : MonoBehaviour
    {
        [Header("Storage Settings")]
        [SerializeField] protected bool _logOperations = true;
        protected List<T> _items = new List<T>();

        public int Count => _items.Count;
        public List<T> Items => new List<T>(_items);

        protected virtual void Awake()
        {
            InitializeStorage();
        }

        protected virtual void InitializeStorage()
        {
            if (_logOperations)
                Debug.Log($"[{GetType().Name}] Storage initialized");
        }

        public virtual void Add(T item)
        {
            _items.Add(item);

            if (_logOperations)
                Debug.Log($"[{GetType().Name}] Added item. Total: {Count}");
        }

        public virtual bool Remove(T item)
        {
            bool removed = _items.Remove(item);

            if (removed && _logOperations)
                Debug.Log($"[{GetType().Name}] Removed item. Total: {Count}");

            return removed;
        }

        public virtual bool RemoveAt(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);

                if (_logOperations)
                    Debug.Log($"[{GetType().Name}] Removed item at index {index}. Total: {Count}");

                return true;
            }
            return false;
        }

        public virtual T Get(int index)
        {
            return index >= 0 && index < _items.Count ? _items[index] : default(T);
        }

        public virtual bool Has(T item)
        {
            return _items.Contains(item);
        }

        public virtual void Clear()
        {
            int clearedCount = _items.Count;
            _items.Clear();

            if (_logOperations)
                Debug.Log($"[{GetType().Name}] Cleared {clearedCount} items");
        }

        public virtual List<T> GetAll()
        {
            return new List<T>(_items);
        }

        [ContextMenu("Log Storage Info")]
        public virtual void LogStorageInfo()
        {
            Debug.Log($"[{GetType().Name}] Storage Info:\n" +
                     $"Items: {Count}\n" +
                     $"Type: {typeof(T).Name}");
        }

        [ContextMenu("Clear Storage")]
        public virtual void DebugClear()
        {
            Clear();
        }
    }
}