using System.Collections.Generic; 
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Interfaces
{
    public interface IItemStorage
    {
        int Count { get; }
        void Add(RewardData rewardData);
        bool Remove(RewardData rewardData);
        void Clear();
        List<RewardData> GetAll(); 
    }
}