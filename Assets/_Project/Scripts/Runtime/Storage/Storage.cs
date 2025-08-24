using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Event.Storage;

namespace _Project.Scripts.Runtime.StorageSystem
{
    public abstract class Storage<T> : MonoBehaviour
    {
        protected List<T> items = new List<T>();

        public int Count => items.Count;

        protected abstract void InitializeStorage();

        protected virtual void Awake()
        {
            InitializeStorage();
        }

        public virtual void Add(T item)
        {
            if (item == null) return;
            
            items.Add(item);
            PublishStorageEvent(StorageChangeType.Added, item);
        }

        public virtual bool Remove(T item)
        {
            if (item == null) return false;
            
            bool removed = items.Remove(item);
            if (removed)
            {
                PublishStorageEvent(StorageChangeType.Removed, item);
            }
            return removed;
        }

        public virtual void Clear()
        {
            items.Clear();
            PublishStorageEvent(StorageChangeType.Cleared);
        }

        public virtual List<T> GetAll()
        {
            return new List<T>(items);
        }

        protected virtual void PublishStorageEvent(StorageChangeType changeType, T changedItem = default)
        {
            PublishStorageEvent(GetAll(), changeType, changedItem);
        }
 
        protected abstract void PublishStorageEvent(List<T> items, StorageChangeType changeType, T changedItem);
    }
}