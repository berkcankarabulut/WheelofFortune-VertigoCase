using _Project.Scripts.Event.Game; 
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement; 

namespace _Project.Scripts.Runtime.Game
{ 
    public class GameController : MonoBehaviour
    {
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

            MessageBroker.Default.Receive<OnExitRequestedEvent>()
                .Subscribe(ExitGame)
                .AddTo(_disposables);
        }

        private void ExitGame(OnExitRequestedEvent exitEvent)
        {
            if (!exitEvent.ConfirmExit) return;
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