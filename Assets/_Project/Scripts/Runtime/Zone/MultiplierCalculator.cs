using _Project.Scripts.Config;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Zone
{
    public class MultiplierCalculator : MonoBehaviour
    {
        private IGameSettings _gameSettings;
        private ReactiveProperty<float> _currentMultiplier = new ReactiveProperty<float>(1f);
        private ReactiveProperty<int> _currentZone = new ReactiveProperty<int>(1);
        private CompositeDisposable _disposables = new CompositeDisposable();

        public IReadOnlyReactiveProperty<float> CurrentMultiplier => _currentMultiplier;
        public IReadOnlyReactiveProperty<int> CurrentZone => _currentZone;

        [Inject]
        public void Construct(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        private void Awake()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void Start()
        {
            UpdateMultiplier(1);
        }

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        {
            UpdateMultiplier(zoneEvent.CurrentZone);
        }

        private void UpdateMultiplier(int zone)
        {
            float multiplier = CalculateZoneMultiplier(zone);
            _currentMultiplier.Value = multiplier;
            _currentZone.Value = zone;
            
            this.Log($"Zone {zone}: Multiplier updated to x{multiplier:F2}");
        }

        // Public method - başka sınıflar spesifik zone için multiplier alabilir
        public float GetMultiplierForZone(int zone)
        {
            return CalculateZoneMultiplier(zone);
        }

        private float CalculateZoneMultiplier(int zone)
        {
            if (zone <= 1) return 1f;

            float multiplier = Mathf.Pow(_gameSettings.ZoneRewardMultiplier, zone - 1);

            if (zone % _gameSettings.SuperZoneInterval == 0)
            {
                multiplier *= _gameSettings.SuperZoneMultiplier;
            }

            return multiplier;
        }

        private void OnDestroy()
        {
            _currentMultiplier?.Dispose();
            _currentZone?.Dispose();
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        [ContextMenu("🔢 Test Multipliers")]
        private void TestMultipliers()
        {
            this.Log("=== MULTIPLIER TEST ===");
            for (int zone = 1; zone <= 35; zone++)
            {
                float multiplier = CalculateZoneMultiplier(zone);
                this.Log($"Zone {zone}: x{multiplier:F2}");
            }
        }

        [ContextMenu("🎯 Compare Old vs New")]
        private void CompareCalculations()
        {
            this.Log("=== OLD vs NEW CALCULATION COMPARISON ===");
            for (int zone = 1; zone <= 10; zone++)
            {
                // New way
                float newMultiplier = GetMultiplierForZone(zone);
                
                // Old way (from original WheelRewardSetter)
                float oldMultiplier = Mathf.Pow(_gameSettings.ZoneRewardMultiplier, zone - 1);
                if (zone % _gameSettings.SuperZoneInterval == 0)
                    oldMultiplier *= _gameSettings.SuperZoneMultiplier;
                
                bool match = Mathf.Approximately(newMultiplier, oldMultiplier);
                this.Log($"Zone {zone}: Old={oldMultiplier:F2}, New={newMultiplier:F2}, Match={match}");
            }
        }
#endif
    }
}