using _Project.Scripts.Config;
using _Project.Scripts.Event.Zone; 
using UniRx;
using UnityEngine; 

namespace _Project.Scripts.Runtime.Zone
{
    // Zone numarasını dinleyip, o zone için matematiksel çarpan hesaplayarak UI'ların otomatik güncellemesi için reactive property sağlar.
    public class MultiplierCalculator : MonoBehaviour
    {  
        private ReactiveProperty<float> _currentMultiplier = new ReactiveProperty<float>(1f);
        private CompositeDisposable _disposables = new CompositeDisposable();

        public IReadOnlyReactiveProperty<float> CurrentMultiplier => _currentMultiplier; 
 
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
            _disposables?.Dispose();
        } 
    }
}