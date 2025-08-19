using _Project.Scripts.Data.Reward; 

namespace _Project.Scripts.Event.Reward
{
    public struct OnRewardCollectedEvent
    {
        public RewardData RewardData { get; }
        
        public OnRewardCollectedEvent(RewardData rewardData)
        {
            RewardData = rewardData;
        }
    }
}