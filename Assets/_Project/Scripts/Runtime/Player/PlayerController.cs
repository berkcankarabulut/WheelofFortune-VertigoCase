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

namespace _Project.Scripts.Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        private ICurrencyManager _currencyManager;
        private CacheItemStorage _rewardStorage;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(ICurrencyManager currencyManager, CacheItemStorage itemStorage)
        {
            _currencyManager = currencyManager;
            _rewardStorage = itemStorage; 
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
                .Subscribe(OnExitRequested)
                .AddTo(_disposables);
        }

        private void OnRewardCollected(OnTryCollectRewardEvent tryCollectRewardEvent)
        { 
            RewardData rewardData = tryCollectRewardEvent.RewardData;
            if (rewardData?.RewardItemSo == null) return;

            if (rewardData.RewardItemSo.Type == RewardType.Bomb)
            {
                OnGameFailed();
            }
            else
            {
                _rewardStorage.Add(rewardData); 
                MessageBroker.Default.Publish(new OnRewardCollectedEvent());
            }
        }

        private void OnExitRequested(OnExitRequestedEvent exitEvent)
        {
            if (exitEvent.ConfirmExit) ExitGame();
        }

        private void ExitGame()
        {
            var currentRewards = _rewardStorage.GetAll();
            var saveEvent = new OnSaveRequestedEvent(currentRewards);

            MessageBroker.Default.Publish(saveEvent); 
        }

        private void OnGameFailed()
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