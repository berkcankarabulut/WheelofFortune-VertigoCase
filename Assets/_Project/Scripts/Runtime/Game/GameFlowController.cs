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
    public class GameFlowController : MonoBehaviour
    {
        private ICurrencyManager _currencyManager;
        private CacheItemStorage _rewardStorage;
        private IZoneManager _zoneManager;
        private IGameSettings _gameSettings; 
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(ICurrencyManager currencyManager, CacheItemStorage itemStorage,
            IZoneManager zoneManager, IGameSettings gameSettings)
        {
            _currencyManager = currencyManager;
            _rewardStorage = itemStorage;
            _zoneManager = zoneManager;
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            InitializeEventSubscriptions();
        }

        private void Start()
        {
            MessageBroker.Default.Publish(new OnGameStartedEvent());
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

            MessageBroker.Default.Receive<OnReviveRequestedEvent>()
                .Subscribe(OnReviveRequested)
                .AddTo(_disposables);
        }

        private void OnRewardCollected(OnRewardCollectedEvent rewardEvent)
        {
            RewardData rewardData = rewardEvent.RewardData;
            if (rewardData?.RewardItemSo == null)
            {
                return;
            }

            if (rewardData.RewardItemSo.Type == RewardType.Bomb)
            { 
                OnGameFailed();
            }
            else
            {
                _rewardStorage.Add(rewardData);
                _zoneManager.NextZone();
            }
        }

        private void OnExitRequested(OnExitRequestedEvent exitEvent)
        {
            if (exitEvent.ConfirmExit)
            {
                ExitGame();
            }
        }

        private void OnReviveRequested(OnReviveRequestedEvent reviveEvent)
        {
            bool spendSuccess = _currencyManager.SpendMoney(_gameSettings.RevivePrice);
            if (!spendSuccess) return;
 
            MessageBroker.Default.Publish(new OnRevivedEvent());
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
            bool canRevive = currentMoney >= _gameSettings.RevivePrice;
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