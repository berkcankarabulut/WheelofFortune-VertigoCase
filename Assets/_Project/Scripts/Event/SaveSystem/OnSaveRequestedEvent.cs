using _Project.Scripts.Data.Reward;
using System.Collections.Generic;

namespace _Project.Scripts.Event.Save
{
    public struct OnSaveRequestedEvent
    {
        public List<RewardData> RewardsToSave { get; } 
        
        public OnSaveRequestedEvent(List<RewardData> rewardsToSave)
        {
            RewardsToSave = rewardsToSave; 
        }
    }
}