using System; 
using UnityEngine; 

namespace _Project.Scripts.Data.Reward
{ 
    //Reward'ların hangi itemi tuttuğunu ve ne kadar tutuğunu belirtme amacı ile kullanılıyor.
    [Serializable]
    public class RewardData
    {
        [SerializeField] private RewardItemSO rewardItemSo;
        [SerializeField] private int _amount = 0; 
        
        public RewardItemSO RewardItemSo => rewardItemSo;
        public int Amount => _amount;

        public RewardData(RewardItemSO rewardItemSo, int amount)
        {
            this.rewardItemSo = rewardItemSo;
            _amount = amount;
        }
    }
}