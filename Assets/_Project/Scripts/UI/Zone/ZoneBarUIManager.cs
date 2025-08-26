using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;
using Zenject;

namespace _Project.Scripts.UI.Zone
{
    public class ZoneBarUIManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private ZoneBarElement zoneBarElementPrefab;
        [SerializeField] private float _offsetPerZone = 80f;
        private int _initialZoneCount = 10;
        private int _batchSize = 10;
        private int _preloadThreshold = 10;

        private IWheelDataService _wheelDataService;
        private List<ZoneBarElement> _zones = new List<ZoneBarElement>();
        private int _nextZoneNumber = 1;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(IWheelDataService wheelDataService)
        {
            _wheelDataService = wheelDataService;
        }

        private void Awake()
        {
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void Start()
        {
            CreateZones(_initialZoneCount);
        }

        private void CreateZones(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var zone = Instantiate(zoneBarElementPrefab, _container);
                var config = _wheelDataService.GetConfigsForZone(_nextZoneNumber).VisualConfig;
                zone.UpdateDisplay(_nextZoneNumber, config.ZoneDisplayColor);

                _zones.Add(zone);
                _nextZoneNumber++;
            }
        }

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        {
            int currentZone = zoneEvent.CurrentZone;

            AddNewZonesIfNeeded(currentZone);
            ScrollToZone(currentZone);
        }

        private void AddNewZonesIfNeeded(int currentZone)
        {
            int lastZoneNumber = _nextZoneNumber - 1;
            int remainingZones = lastZoneNumber - currentZone;

            if (remainingZones <= _preloadThreshold)
            {
                CreateZones(_batchSize);
            }
        }

        private void ScrollToZone(int zoneNumber)
        {
            int firstZoneNumber = _nextZoneNumber - _zones.Count;
            int zoneIndex = zoneNumber - firstZoneNumber;

            if (zoneIndex < 0 || zoneIndex >= _zones.Count) return;

            float targetX = -zoneIndex * _offsetPerZone;
            _container.DOAnchorPosX(targetX, 0.4f)
                .SetEase(Ease.OutCubic)
                .SetId("ZoneBarScroll");
        }

        private void OnDestroy()
        {
            DOTween.Kill("ZoneBarScroll");
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _initialZoneCount = Mathf.Max(1, _initialZoneCount);
            _batchSize = Mathf.Max(1, _batchSize);
            _preloadThreshold = Mathf.Max(1, Mathf.Min(_preloadThreshold, _batchSize / 2));
        }
#endif
    }
}