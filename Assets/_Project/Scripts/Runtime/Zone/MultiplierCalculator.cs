using _Project.Scripts.Config;
using _Project.Scripts.Event.Zone; 
using UniRx;
using UnityEngine; 

namespace _Project.Scripts.Runtime.Zone
{
    public class MultiplierCalculator : MonoBehaviour
    { 
        private ReactiveProperty<float> _currentMultiplier = new ReactiveProperty<float>(1f);
        private ReactiveProperty<int> _currentZone = new ReactiveProperty<int>(1);
        private CompositeDisposable _disposables = new CompositeDisposable();

        public IReadOnlyReactiveProperty<float> CurrentMultiplier => _currentMultiplier;
        public IReadOnlyReactiveProperty<int> CurrentZone => _currentZone;
 
        private void Awake()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void Start() => UpdateMultiplier(1);

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent) => UpdateMultiplier(zoneEvent.CurrentZone);

        private void UpdateMultiplier(int zone)
        {
            float multiplier = GetMultiplierForZone(zone);
            _currentMultiplier.Value = multiplier;
            _currentZone.Value = zone; 
        }

        public float GetMultiplierForZone(int zone)
        {
            if (zone <= 1) return 1f;

            float multiplier = Mathf.Pow(GameSettings.ZONE_MULTIPLIER, zone - 1); 
            return multiplier;
        }

        private void OnDestroy()
        {
            _currentMultiplier?.Dispose();
            _currentZone?.Dispose();
            _disposables?.Dispose();
        } 
    }
}