using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Service;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Zone;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelRewardSetter : MonoBehaviour, IWheelRewardSetter
    {
        [Header("Reward UI References")] [SerializeField]
        private WheelRewardUIElement[] _wheelRewardUIs;

        private IWheelDataService _wheelDataService;
        private MultiplierCalculator _multiplierCalculator;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            IWheelDataService wheelDataService,
            MultiplierCalculator multiplierCalculator)
        {
            _wheelDataService = wheelDataService;
            _multiplierCalculator = multiplierCalculator;
        }

        private void Awake()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        public void OnZoneChanged(OnZoneChangedEvent zoneChangedEvent)
        {
            LoadRewards(zoneChangedEvent.CurrentZone);
        }

        public void LoadRewards(int zone)
        {
            WheelDataSO wheelData = _wheelDataService.GetConfigsForZone(zone);
            SetRewardsForWheel(wheelData, _wheelDataService.GetBombReward());
        }

        private void SetRewardsForWheel(WheelDataSO wheelData, RewardData bombReward)
        {
            if (!wheelData || !wheelData.HasRewards || _wheelRewardUIs?.Length == 0) return;

            bool shouldAddBomb = wheelData.Type == WheelType.BronzeZone;
            int bombSliceIndex = shouldAddBomb ? Random.Range(0, _wheelRewardUIs.Length) : -1;
            float currentMultiplier = _multiplierCalculator.CurrentMultiplier.Value;
 
            List<RewardData> cacheData = new List<RewardData>(); 
            RewardData[] originalPool = wheelData.RewardDataPool;
    
            while (cacheData.Count < _wheelRewardUIs.Length)
                cacheData.AddRange(originalPool);
 
            for (int i = 0; i < _wheelRewardUIs.Length; i++)
            {
                int randomIndex = Random.Range(0, cacheData.Count);
                RewardData rewardData = cacheData[randomIndex];
                cacheData.RemoveAt(randomIndex);

                rewardData = (i == bombSliceIndex) ? bombReward : GetScaledReward(rewardData, currentMultiplier);
                _wheelRewardUIs[i]?.SetRewardData(rewardData);
            }
        } 

        private RewardData GetScaledReward(RewardData baseReward, float multiplier)
        {
            if (baseReward == null) return null;
            int scaledAmount = Mathf.RoundToInt(baseReward.Amount * multiplier);
            scaledAmount = Mathf.Clamp(scaledAmount, 0, baseReward.RewardItemSo.MaxLimit);
            return new RewardData(baseReward.RewardItemSo, scaledAmount);
        }
    }
}