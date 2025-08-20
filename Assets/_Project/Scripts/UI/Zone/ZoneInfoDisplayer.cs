using _Project.Scripts.Config;
using _Project.Scripts.Event.Zone;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.UI.Zone
{
    public class ZoneInfoDisplayer : MonoBehaviour
    {
        [Header("Zone Texts")] 
        [SerializeField] private TextMeshProUGUI _safeZoneText;
        [SerializeField] private TextMeshProUGUI _superZoneText;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private int _nextSafeZone;
        private int _nextSuperZone;

        private void Awake()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void Start()
        { 
            InitializeNextZones(1);
            UpdateZoneDisplays();
        }

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        {
            int currentZone = zoneEvent.CurrentZone; 
            UpdateNextZones(currentZone);
            UpdateZoneDisplays();
        }

        private void InitializeNextZones(int currentZone)
        {
            _nextSafeZone = GetNextSafeZone(currentZone);
            _nextSuperZone = GetNextSuperZone(currentZone);
        }

        private void UpdateNextZones(int currentZone)
        { 
            _nextSafeZone = GetNextSafeZone(currentZone); 
            _nextSuperZone = GetNextSuperZone(currentZone);
        }

        private int GetNextSafeZone(int currentZone)
        { 
            int nextSafe = ((currentZone / GameSettings.SAFE_ZONE_INTERVAL) + 1) * GameSettings.SAFE_ZONE_INTERVAL; 
            if (currentZone % GameSettings.SAFE_ZONE_INTERVAL == 0)
            {
                nextSafe = currentZone + GameSettings.SAFE_ZONE_INTERVAL;
            }
            
            return nextSafe;
        }

        private int GetNextSuperZone(int currentZone)
        { 
            int nextSuper = ((currentZone / GameSettings.SUPER_ZONE_INTERVAL) + 1) * GameSettings.SUPER_ZONE_INTERVAL; 
            if (currentZone % GameSettings.SUPER_ZONE_INTERVAL == 0)
            {
                nextSuper = currentZone + GameSettings.SUPER_ZONE_INTERVAL;
            }
            
            return nextSuper;
        }

        private void UpdateZoneDisplays()
        {
            if (_safeZoneText != null)
                _safeZoneText.text = _nextSafeZone.ToString();

            if (_superZoneText != null)
                _superZoneText.text = _nextSuperZone.ToString();
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        } 
    }
}