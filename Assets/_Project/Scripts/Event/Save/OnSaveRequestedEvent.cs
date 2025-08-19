using _Project.Scripts.Data.Reward;
using System.Collections.Generic;

namespace _Project.Scripts.Event.Save
{
    public struct OnSaveRequestedEvent
    {
        public List<ItemAmountData> RewardsToSave { get; } 
        
        public OnSaveRequestedEvent(List<ItemAmountData> rewardsToSave)
        {
            RewardsToSave = rewardsToSave; 
        }
    }
}