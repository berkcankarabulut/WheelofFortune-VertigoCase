using UnityEngine;

namespace _Project.Scripts.UI.Storage
{
    // T tipinde veri gösteren storage UI elementleri için base sınıf
    public abstract class StorageUIElement<T> : MonoBehaviour
    {
        protected T _data;
        public T Data => _data;

        public virtual void SetData(T data)
        {
            _data = data;
            UpdateDisplay();
        }

        protected abstract void UpdateDisplay();

        public virtual void ResetUI()
        {
            _data = default(T);
            ClearDisplay();
        }

        protected abstract void ClearDisplay();
    }
}