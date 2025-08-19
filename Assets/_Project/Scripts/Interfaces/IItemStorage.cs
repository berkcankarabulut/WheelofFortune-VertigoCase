using System.Collections.Generic;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Interfaces
{
    public interface IItemStorage
    {
        int Count { get; }
        void Add(RewardData rewardData);
        bool RemoveReward(RewardItemSO rewardItem, int amount);
        void Clear();
        List<RewardData> GetAll();
        int GetTotalAmount(RewardItemSO rewardItem);
        List<RewardData> GetRewardsByType(RewardType rewardType);
    }
}