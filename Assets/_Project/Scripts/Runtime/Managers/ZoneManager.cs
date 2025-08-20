using _Project.Scripts.Event.Reward;
using UnityEngine;
using UniRx;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;

namespace _Project.Scripts.Runtime.Wheel
{
    public class ZoneManager : MonoBehaviour, IZoneManager
    {
        private int _currentZone = 1;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            MessageBroker.Default.Receive<OnRewardCollectedEvent>()
                .Subscribe(OnExitRequested)
                .AddTo(_disposables);
        }

        private void OnExitRequested(OnRewardCollectedEvent onRewardCollectedEvent)
        {
            NextZone();
        }

        private void Start()
        {
            MessageBroker.Default.Publish(new OnZoneChangedEvent(_currentZone));
        }

        public void NextZone()
        {
            _currentZone++;
            PublishZoneChanged(_currentZone);
        }

        private void PublishZoneChanged(int zone) => MessageBroker.Default.Publish(new OnZoneChangedEvent(zone));
    }
}