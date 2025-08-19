using UnityEngine;
using DG.Tweening;
using TMPro;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Service;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Wheel
{
    public class ZoneCenterController : MonoBehaviour
    { 
        [Header("UI Components")] 
        [SerializeField] private TextMeshProUGUI _zone_center_value; 
        [SerializeField] private Image _zone_center_Background;  

        private Tween _glowTween;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(SetZone)
                .AddTo(_disposables);
        }

        public void SetZone(OnZoneChangedEvent zone)
        {
            WheelVisualConfig visualConfig = WheelDataService.Instance.GetConfigsForZone(zone.CurrentZone).VisualConfig;

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
                .SetEase(Ease.OutElastic);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}