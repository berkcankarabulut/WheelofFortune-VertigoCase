using System;
using _Project.Scripts.Data.Item;
using UnityEngine; 

namespace _Project.Scripts.Data.Reward
{ 
    [Serializable]
    public class ItemAmountData
    {
        [SerializeField] private ItemSO itemSo;
        [SerializeField] private int _amount = 0; 
        
        public ItemSO ItemSo => itemSo;
        public int Amount => _amount;

        public ItemAmountData(ItemSO itemSo, int amount)
        {
            this.itemSo = itemSo;
            _amount = amount;
        }
    }
}