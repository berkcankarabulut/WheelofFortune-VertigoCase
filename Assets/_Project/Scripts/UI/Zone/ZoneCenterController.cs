using UnityEngine;
using DG.Tweening;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Service;
using UniRx;
using Zenject;

namespace _Project.Scripts.UI.Zone
{
    public class ZoneCenterController : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private TMPro.TextMeshProUGUI _zone_center_value;

        [SerializeField] private UnityEngine.UI.Image _zone_center_Background;

        private IWheelDataService _wheelDataService;
        private Tween _glowTween;
        private CompositeDisposable _disposables = new UniRx.CompositeDisposable();

        [Inject]
        public void Construct(IWheelDataService wheelDataService)
        {
            _wheelDataService = wheelDataService;
        }

        private void Awake()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(SetZone)
                .AddTo(_disposables);
        } 
        
        private void SetZone(OnZoneChangedEvent zone)
        {
            WheelVisualConfig visualConfig = _wheelDataService.GetConfigsForZone(zone.CurrentZone).VisualConfig;

            if (_zone_center_value != null)
                _zone_center_value.text = (zone.CurrentZone).ToString();
            if (_zone_center_Background != null)
            {
                _zone_center_Background.sprite = visualConfig.ZoneBackground;
            }

            AnimateZoneTypeChange();
        }

        private void AnimateZoneTypeChange()
        {
            transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 8, 0.5f)
                .SetEase(DG.Tweening.Ease.OutElastic);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}