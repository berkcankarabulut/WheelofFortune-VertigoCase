using _Project.Scripts.Config;
using _Project.Scripts.Event.Zone;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Zone
{
    public class ZoneInfoDisplayer : MonoBehaviour
    {
        [Header("Zone Texts")]
        [SerializeField] private TextMeshProUGUI _safeZoneText;
        [SerializeField] private TextMeshProUGUI _superZoneText;

        private IGameSettings _gameSettings;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        private void Start()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(e => UpdateZoneDisplays(e.CurrentZone))
                .AddTo(_disposables);
        }

        private void UpdateZoneDisplays(int currentZone)
        {
            if (_safeZoneText != null)
                _safeZoneText.text = GetNextZone(currentZone, _gameSettings.SafeZoneInterval).ToString();
            
            if (_superZoneText != null)
                _superZoneText.text = GetNextZone(currentZone, _gameSettings.SuperZoneInterval).ToString();
        }

        private int GetNextZone(int currentZone, int interval)
        {
            return ((currentZone / interval) + 1) * interval;
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}