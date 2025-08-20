using _Project.Scripts.Config;
using _Project.Scripts.Event.Zone;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.UI.Zone
{
    public class ZoneInfoDisplayer : MonoBehaviour
    {
        [Header("Zone Texts")] [SerializeField]
        private TextMeshProUGUI _safeZoneText;

        [SerializeField] private TextMeshProUGUI _superZoneText;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private int _nextSafeZone;
        private int _nextSuperZone;

        private void Awake()
        {
            _nextSafeZone = GameSettings.SAFE_ZONE_INTERVAL;
            _nextSuperZone = GameSettings.SUPER_ZONE_INTERVAL;

            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void Start()
        {
            UpdateZoneDisplays();
        }

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        {
            int currentZone = zoneEvent.CurrentZone;
            if (currentZone % GameSettings.SUPER_ZONE_INTERVAL == 0)
                _nextSuperZone = GetNextSuperZone(currentZone);
            else if (currentZone % GameSettings.SAFE_ZONE_INTERVAL == 0)
                _nextSafeZone = GetNextSafeZone(currentZone);

            UpdateZoneDisplays();
        }

        private int GetNextSafeZone(int currentZone)
        {
            return ((currentZone / GameSettings.SAFE_ZONE_INTERVAL) + 1) * GameSettings.SAFE_ZONE_INTERVAL;
        }

        private int GetNextSuperZone(int currentZone)
        {
            return ((currentZone / GameSettings.SUPER_ZONE_INTERVAL) + 1) * GameSettings.SUPER_ZONE_INTERVAL;
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