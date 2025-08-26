using System.Collections.Generic;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Event.Save
{
    public class OnSaveRequestedEvent
    {
        public List<RewardData> CacheItems { get; }

        public OnSaveRequestedEvent(List<RewardData> cacheItems = null)
        {
            CacheItems = cacheItems ?? new List<RewardData>();
        }
    }
}