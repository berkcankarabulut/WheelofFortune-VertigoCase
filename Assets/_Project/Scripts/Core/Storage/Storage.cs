using System;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Core.Storage
{
    public abstract class Storage<T> : MonoBehaviour
    {  
        protected List<T> _items = new List<T>();

        public int Count => _items.Count;
        public List<T> Items => new List<T>(_items);
        public Action<T> OnAdded;
        public Action<T> OnRemoved;

        protected abstract void InitializeStorage();
        

        public virtual void Add(T item)
        {
            _items.Add(item);
            OnAdded?.Invoke(item); 
        }

        public virtual bool Remove(T item)
        {
            bool removed = _items.Remove(item);
             
            OnRemoved?.Invoke(item);
            return removed;
        }    

        public virtual void Clear()
        { 
            _items.Clear(); 
        }

        public virtual List<T> GetAll()
        {
            return new List<T>(_items);
        } 
    }
}