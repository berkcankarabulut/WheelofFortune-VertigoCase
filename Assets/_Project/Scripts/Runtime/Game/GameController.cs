using _Project.Scripts.Config;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Interfaces;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Runtime.Game
{ 
    public class GameController : MonoBehaviour
    {
        [Inject] private ICurrencyManager _currencyManager;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            InitializeEventSubscriptions();
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            MessageBroker.Default.Publish(new OnGameStartEvent());
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnGameOveredEvent>()
                .Subscribe(OnGiveUp)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnSafeExitRequestedEvent>()
                .Subscribe(ExitGame)
                .AddTo(_disposables);
            
            MessageBroker.Default.Receive<OnSpinIndicatorOnBombEvent>()
                .Subscribe(_ => OnLootBomb())
                .AddTo(_disposables);
        }

        private void OnLootBomb()
        {
            int currentMoney = _currencyManager.GetMoney();
            bool canRevive = currentMoney >= GameSettings.REVIVE_PRICE;
            MessageBroker.Default.Publish(new OnGameFailedEvent(canRevive));
        }
        
        private void ExitGame(OnSafeExitRequestedEvent safeExitEvent)
        {
            if (!safeExitEvent.ConfirmExit) return;
            DOVirtual.DelayedCall(0.5f, GameReset);
        }

        private void OnGiveUp(OnGameOveredEvent gameOverEvent)
        {
            GameReset();
        }

        private void GameReset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}