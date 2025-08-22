using System.Collections.Generic;

namespace _Project.Scripts.Event.Storage
{ 
    public class OnStorageChangedEvent<TStorage, TData>
    {
        public List<TData> Items { get; }
        public StorageChangeType ChangeType { get; }
        public TData ChangedItem { get; }

        public OnStorageChangedEvent(List<TData> items, StorageChangeType changeType = StorageChangeType.Updated, TData changedItem = default)
        {
            Items = items ?? new List<TData>();
            ChangeType = changeType;
            ChangedItem = changedItem;
        }
    }

    // Storage change types for better event handling
    public enum StorageChangeType
    {
        Added,
        Removed,
        Updated,
        Cleared,
        Loaded
    }
}