using _Project.Scripts.Event.Game;
using _Project.Scripts.Interfaces; 
using _Project.Scripts.Runtime.Wheel;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Runtime.Game
{
    public class GameController : MonoBehaviour
    { 
        private CompositeDisposable _disposables = new CompositeDisposable();  
        
        private void Awake()
        {
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        {   
            MessageBroker.Default.Receive<OnGameOveredEvent>()
                .Subscribe(OnGiveUp)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnExitRequestedEvent>()
                .Subscribe(OnExitRequested)
                .AddTo(_disposables);
        } 
        
        private void OnExitRequested(OnExitRequestedEvent exitEvent)
        {
            if (exitEvent.ConfirmExit) ExitGame();
        }

        private void ExitGame()
        {  
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