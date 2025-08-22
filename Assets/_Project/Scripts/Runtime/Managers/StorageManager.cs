using System.Collections.Generic;
using _Project.Scripts.Config;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Reward;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Storage;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Managers
{
    //Reward toplama/bomb kontrolü yapıp, cache ve storage koordinasyonunu sağlar. 
    public class StorageManager : MonoBehaviour
    {
        private ICurrencyManager _currencyManager;
        private IItemStorage _cacheItemStorage;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(ICurrencyManager currencyManager, CacheItemStorage item)
        {
            _currencyManager = currencyManager;
            _cacheItemStorage = item;
        }

        private void Awake()
        {
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnTryCollectRewardEvent>()
                .Subscribe(OnRewardCollected)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnExitRequestedEvent>()
                .Subscribe(OnExitGame)
                .AddTo(_disposables);
        }

        private void OnRewardCollected(OnTryCollectRewardEvent tryCollectRewardEvent)
        {
            RewardData rewardData = tryCollectRewardEvent.RewardData;
            if (rewardData?.RewardItemSo == null) return;

            if (rewardData.RewardItemSo.Type == RewardType.Bomb)
            {
                OnLootBomb();
            }
            else
            {
                _cacheItemStorage.Add(rewardData);
                MessageBroker.Default.Publish(new OnRewardCollectedEvent());
            }
        }

        private void OnExitGame(OnExitRequestedEvent exitEvent)
        {
            if (!exitEvent.ConfirmExit) return;
            List<RewardData> currentRewards = _cacheItemStorage.GetAll();
            OnSaveRequestedEvent saveEvent = new OnSaveRequestedEvent(currentRewards);

            MessageBroker.Default.Publish(saveEvent);
        }

        private void OnLootBomb()
        {
            int currentMoney = _currencyManager.GetMoney();
            bool canRevive = currentMoney >= GameSettings.REVIVE_PRICE;
            MessageBroker.Default.Publish(new OnGameFailedEvent(canRevive));
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}