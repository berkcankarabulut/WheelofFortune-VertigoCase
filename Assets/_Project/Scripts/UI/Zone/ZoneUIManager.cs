using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using UniRx;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Service;

namespace _Project.Scripts.UI.Wheel
{
    public class ZoneUIManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private ZoneUIElement _zoneUIElementPrefab;
        [SerializeField] private float _offsetPerZone = 80f;
        [SerializeField] private int _visibleZoneCount = 10;

        private ObjectPool<ZoneUIElement> _zonePool;
        private List<ZoneUIElement> _activeZones = new List<ZoneUIElement>();
        private int _firstVisibleZone = 1;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            _zonePool = new ObjectPool<ZoneUIElement>(
                () => Instantiate(_zoneUIElementPrefab, _container),
                e =>
                {
                    e.transform.SetParent(_container);
                    e.gameObject.SetActive(true);
                },
                e =>
                {
                    e.transform.SetParent(null);
                    e.gameObject.SetActive(false);
                },
                e =>
                {
                    if (e) Destroy(e.gameObject);
                },
                maxSize: _visibleZoneCount
            );

            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(e => OnZoneChanged(e.CurrentZone))
                .AddTo(_disposables);
        }

        private void Start()
        {
            for (int i = 0; i < _visibleZoneCount; i++)
                AddZone(_firstVisibleZone + i, i);
        }

        private void OnZoneChanged(int currentZone)
        {
            if (currentZone >= _firstVisibleZone + _visibleZoneCount - (_visibleZoneCount / 2))
                UpdateZones();

            _container.DOAnchorPosX(_container.anchoredPosition.x - _offsetPerZone, 0.4f)
                .SetEase(Ease.OutCubic);
        }

        private void UpdateZones()
        { 
            for (int i = 0; i < 3 && _activeZones.Count > 0; i++)
            {
                _zonePool.Release(_activeZones[0]);
                _activeZones.RemoveAt(0);
            } 
            _firstVisibleZone += 3; 
            
            for (int i = 0; i < 3; i++)
                AddZone(_firstVisibleZone + _activeZones.Count + i, _activeZones.Count + i);
 
            var pos = _container.anchoredPosition;
            _container.anchoredPosition = new Vector2(pos.x + _offsetPerZone * 3, pos.y);
        }

        private void AddZone(int zoneNumber, int position)
        {
            var zone = _zonePool.Get();
            _activeZones.Add(zone);

            var config = WheelDataService.Instance.GetConfigsForZone(zoneNumber).VisualConfig;
            zone.UpdateDisplay(zoneNumber, config.ZoneDisplayColor);
            zone.GetComponent<RectTransform>().anchoredPosition = new Vector2(position * _offsetPerZone, 0);
        }

        private void OnDestroy()
        {
            if (_zonePool != null)
            {
                _activeZones?.ForEach(z => _zonePool.Release(z));
                _zonePool?.Dispose(); 
            }
            _disposables?.Dispose();
        }
    }
}