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

        private int _currentSafeZone;
        private int _currentSuperZone;
  
        private void Awake()
        {
            _currentSafeZone = GameSettings.SAFE_ZONE_INTERVAL;
            _currentSuperZone = GameSettings.SUPER_ZONE_INTERVAL;

            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void Start() => UpdateZoneDisplays();

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        {
             int zone = zoneEvent.CurrentZone;

            if (zone % _currentSuperZone == 0) _currentSuperZone += GameSettings.SUPER_ZONE_INTERVAL;
            else if (zone % _currentSafeZone == 0)
            {
                _currentSafeZone += (zone == _currentSuperZone - 5)
                    ? GameSettings.SAFE_ZONE_INTERVAL * 2
                    : GameSettings.SAFE_ZONE_INTERVAL;
            }

            UpdateZoneDisplays();
        }

        private void UpdateZoneDisplays()
        {
            _safeZoneText.text = _currentSafeZone.ToString();
            _superZoneText.text = _currentSuperZone.ToString();
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}