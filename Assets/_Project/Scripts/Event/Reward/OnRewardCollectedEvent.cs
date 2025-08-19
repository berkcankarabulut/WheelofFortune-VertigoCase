using _Project.Scripts.Data.Reward; 

namespace _Project.Scripts.Event.Reward
{
    public struct OnRewardCollectedEvent
    {
        public ItemAmountData ItemAmountData { get; }
        
        public OnRewardCollectedEvent(ItemAmountData itemAmountData)
        {
            ItemAmountData = itemAmountData;
        }
    }
}