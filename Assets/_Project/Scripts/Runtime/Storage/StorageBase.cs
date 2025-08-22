using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using _Project.Scripts.Event.Storage;

namespace _Project.Scripts.Core.Storage
{
    public abstract class Storage<T> : MonoBehaviour
    {
        protected List<T> _items = new List<T>();

        public int Count => _items.Count;

        protected abstract void InitializeStorage();

        protected virtual void Awake()
        {
            InitializeStorage();
        }

        public virtual void Add(T item)
        {
            if (item == null) return;
            
            _items.Add(item);
            PublishStorageEvent(StorageChangeType.Added, item);
        }

        public virtual bool Remove(T item)
        {
            if (item == null) return false;
            
            bool removed = _items.Remove(item);
            if (removed)
            {
                PublishStorageEvent(StorageChangeType.Removed, item);
            }
            return removed;
        }

        public virtual void Clear()
        {
            _items.Clear();
            PublishStorageEvent(StorageChangeType.Cleared);
        }

        public virtual List<T> GetAll()
        {
            return new List<T>(_items);
        }

        protected virtual void PublishStorageEvent(StorageChangeType changeType, T changedItem = default)
        {
            PublishStorageEvent(GetAll(), changeType, changedItem);
        }
 
        protected abstract void PublishStorageEvent(List<T> items, StorageChangeType changeType, T changedItem);
    }
}