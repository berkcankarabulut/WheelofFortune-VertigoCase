using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Reward;
using UnityEngine;
using UniRx;
using _Project.Scripts.Event.Zone; 

namespace _Project.Scripts.Runtime.Wheel
{
    // Reward toplandığında zone'u bir artırıp, zone değişiklik event'i yayınlar.
    public class ZoneManager : MonoBehaviour
    {
        private int _currentZone = 1;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            MessageBroker.Default.Receive<OnRewardCollectedEvent>()
                .Subscribe(OnExitRequested)
                .AddTo(_disposables);
            MessageBroker.Default.Receive<OnGameStartEvent>()
                .Subscribe(_ => OnStartRequested())
                .AddTo(_disposables);
        } 
        
        private void OnStartRequested()
        {
            PublishZoneChanged(_currentZone);
        }
        
        private void OnExitRequested(OnRewardCollectedEvent onRewardCollectedEvent)
        {
            NextZone();
        }

        public void NextZone()
        {
            _currentZone++;
            PublishZoneChanged(_currentZone);
        }

        private void PublishZoneChanged(int zone) => MessageBroker.Default.Publish(new OnZoneChangedEvent(zone));
    }
}