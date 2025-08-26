using System;
using System.Collections.Generic;

namespace _Project.Scripts.Runtime.StorageSystem
{
    // Item'ların save'leri tutmayı sağlayan class.
    [Serializable]
    public class ItemSaveData
    {
        [Serializable]
        public class SavedItem
        {
            public string itemId;
            public int amount;
        }

        public List<SavedItem> items = new List<SavedItem>();
        public string saveTime;

        public ItemSaveData()
        {
            saveTime = DateTime.Now.ToString();
        }
    }
}