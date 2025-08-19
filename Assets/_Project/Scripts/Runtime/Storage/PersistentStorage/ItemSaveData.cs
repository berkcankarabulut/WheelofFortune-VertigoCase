using System;
using System.Collections.Generic;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Runtime.Storage
{
    [Serializable]
    public class ItemSaveData
    {
        public List<RewardData> rewards = new List<RewardData>();
        public string saveTime;

        public ItemSaveData()
        {
            saveTime = DateTime.Now.ToString();
        }
    }
}