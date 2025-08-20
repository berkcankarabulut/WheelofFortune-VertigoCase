using System.Collections.Generic;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Event.Storage
{
    public struct OnCacheStorageChangedEvent
    {
        public List<RewardData> CacheData { get; }
        
        public OnCacheStorageChangedEvent(List<RewardData> cacheData)
        {
            CacheData = cacheData;
        }
    }
}