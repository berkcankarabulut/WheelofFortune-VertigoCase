using System.Collections.Generic;
using UnityEngine; 
using DG.Tweening;
using UniRx;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Service; 
using Zenject;

namespace _Project.Scripts.UI.Zone
{
    public class ZoneBarUIManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private ZoneBarElement zoneBarElementPrefab;
        [SerializeField] private float _offsetPerZone = 80f;
        [SerializeField] private int _initialZoneCount = 50;  
        [SerializeField] private int _batchSize = 25; 
        [SerializeField] private int _preloadThreshold = 10; 

        private IWheelDataService _wheelDataService;
        private List<ZoneBarElement> _allZones = new List<ZoneBarElement>();
        private int _currentMaxZone = 0;
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
            CreateInitialZones();
        }

        private void CreateInitialZones()
        {
            CreateZoneBatch(1, _initialZoneCount); 
        }

        private void CreateZoneBatch(int startZone, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int zoneNumber = startZone + i;
                CreateSingleZone(zoneNumber);
            }

            _currentMaxZone = startZone + count - 1; 
        }

        private void CreateSingleZone(int zoneNumber)
        {
            var zone = Instantiate(zoneBarElementPrefab, _container);
            var config = _wheelDataService.GetConfigsForZone(zoneNumber).VisualConfig;
            zone.UpdateDisplay(zoneNumber, config.ZoneDisplayColor);
 
            var rectTransform = zone.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2((zoneNumber - 1) * _offsetPerZone, 0);

            _allZones.Add(zone);
        }

        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        {
            int currentZone = zoneEvent.CurrentZone; 
            CheckAndAddNewZones(currentZone);
 
            ScrollToZone(currentZone);
        }

        private void CheckAndAddNewZones(int currentZone)
        { 
            int remainingZones = _currentMaxZone - currentZone;

            if (remainingZones <= _preloadThreshold)
            {
                int newStartZone = _currentMaxZone + 1;
                CreateZoneBatch(newStartZone, _batchSize); 
            }
        }

        private void ScrollToZone(int zoneNumber)
        { 
            float targetX = -(zoneNumber - 1) * _offsetPerZone;

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
            if (_initialZoneCount <= 0)
                _initialZoneCount = 50;

            if (_batchSize <= 0)
                _batchSize = 25;

            if (_preloadThreshold <= 0)
                _preloadThreshold = 10;

            if (_preloadThreshold >= _batchSize)
                _preloadThreshold = _batchSize / 2;
        }
#endif
    }
}