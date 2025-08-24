using _Project.Scripts.Data.Reward; 

namespace _Project.Scripts.Event.Reward
{
    public struct OnTryCollectRewardEvent
    {
        public RewardData RewardData { get; }
        
        public OnTryCollectRewardEvent(RewardData rewardData)
        {
            RewardData = rewardData;
        } 
    }
}