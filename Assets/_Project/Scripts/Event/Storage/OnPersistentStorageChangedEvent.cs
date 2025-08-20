using System.Collections.Generic;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Event.Storage
{ 
    public struct OnPersistentStorageChangedEvent
    {
        public List<RewardData> PersistentData { get; }
        
        public OnPersistentStorageChangedEvent(List<RewardData> persistentData)
        {
            PersistentData = persistentData;
        }
    }
}