using _Project.Scripts.Config;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Reward;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Storage;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Runtime.Game
{
    public class GameController : MonoBehaviour
    {
        private ICurrencyManager _currencyManager;
        private CacheItemStorage _rewardStorage;
        private IZoneManager _zoneManager;
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
            MessageBroker.Default.Receive<OnRewardCollectedEvent>()
                .Subscribe(OnRewardCollected)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnGameOveredEvent>()
                .Subscribe(OnGiveUp)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnExitRequestedEvent>()
                .Subscribe(OnExitRequested)
                .AddTo(_disposables);
        }

        private void OnRewardCollected(OnRewardCollectedEvent rewardEvent)
        {
            RewardData rewardData = rewardEvent.RewardData;
            if (rewardData?.RewardItemSo == null) return;

            if (rewardData.RewardItemSo.Type == RewardType.Bomb)
            {
                OnGameFailed();
            }
            else
            {
                _rewardStorage.Add(rewardData);
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
            DOVirtual.DelayedCall(0.5f, GameReset);
        }

        private void OnGameFailed()
        {
            int currentMoney = _currencyManager.GetMoney();
            bool canRevive = currentMoney >= GameSettings.REVIVE_PRICE;
            MessageBroker.Default.Publish(new OnGameFailedEvent(canRevive));
        }

        private void OnGiveUp(OnGameOveredEvent gameOverEvent)
        {
            GameReset();
        }

        private void GameReset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}