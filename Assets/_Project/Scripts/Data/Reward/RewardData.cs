using System;
using _Project.Scripts.Data.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Data.Reward
{ 
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