using _Project.Scripts.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;
using _Project.Scripts.Event.Reward;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Runtime.Managers;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.Runtime.Wheel;
using _Project.Scripts.Utils; 

namespace _Project.Scripts.Runtime.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private CurrencyManager _currencyManager;
        [SerializeField] private CacheRewardStorage _rewardStorage;
        [SerializeField] private ZoneManager _zoneManager;
        private CompositeDisposable _disposables = new CompositeDisposable();
        
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
        }
        
        private void OnRewardCollected(OnRewardCollectedEvent rewardEvent)
        {
            ItemAmountData itemAmountData = rewardEvent.ItemAmountData;

            if (itemAmountData?.ItemSo == null)
            { 
                return;
            }
            
            if (itemAmountData.ItemSo.Type == RewardType.Bomb)
            { 
                _rewardStorage.Clear();
                OnGameFailed();
            }
            else
            { 
                _rewardStorage.Add(itemAmountData); 
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

        private void ExitGame()
        { 
            var currentRewards = _rewardStorage.GetAll();
            var saveEvent = new OnSaveRequestedEvent(currentRewards);
            
            MessageBroker.Default.Publish(saveEvent); 
            DOVirtual.DelayedCall(0.5f, () =>
            { 
                GameReset(); 
            });
        }

        private void OnGameFailed()
        {
            // CurrencyManager'dan mevcut para miktarını al
            int currentMoney = _currencyManager.GetMoney();
            print("currentMoney:"+currentMoney);
            // GameSettings'teki RevivePrice ile karşılaştır
            bool canRevive = currentMoney >= GameSettings.RevivePrice;
            print("canRevive:"+canRevive);
            MessageBroker.Default.Publish(new OnGameFailedEvent(canRevive));

            this.Log($"💣 Game Failed! Bomb collected, all rewards lost.");
        }


        private void OnGiveUp(OnGameOveredEvent gameOverEvent)
        {
            GameReset();
        }

        private void OnRevive()
        { 
            _zoneManager.NextZone();
        }

        private void GameReset()
        { 
            this.Log("🔄 Restarting game...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void OnDestroy()
        {
            DOTween.Kill("GameRestart");
            _disposables?.Dispose();
        }
    }
}