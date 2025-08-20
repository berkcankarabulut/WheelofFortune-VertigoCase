using UnityEngine;
using DG.Tweening;
using UniRx; 
using _Project.Scripts.Event.Wheel;
using _Project.Scripts.Interfaces;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelSpinner : MonoBehaviour, IWheelSpinner
    {
        [SerializeField] private Transform _wheelTransform;
        [SerializeField] private float _spinDuration = 3f;
        [SerializeField] private Ease _spinEase = Ease.OutQuart;
        [SerializeField] private int _minRotations = 5;
        [SerializeField] private int _slotCount = 8;
        
        private bool _isSpinning = false;
        private CompositeDisposable _disposables = new CompositeDisposable(); 
        public bool IsSpinning => _isSpinning;
            
        private void Awake()
        {
            MessageBroker.Default.Receive<OnRequestWheelSpinStartEvent>()
                .Subscribe(_ => StartSpin())
                .AddTo(_disposables);
        }

        private void StartSpin()
        {
            if (_isSpinning) return;
    
            _isSpinning = true; 
            int randomSegment = Random.Range(0, _slotCount);
            float segmentAngle = 360f / _slotCount; 
     
            float totalRotation = 360f * _minRotations + randomSegment * segmentAngle;
    
            MessageBroker.Default.Publish(new OnWheelSpinStartEvent());
    
            _wheelTransform.DORotate(new Vector3(0, 0, -totalRotation), _spinDuration, RotateMode.LocalAxisAdd)
                .SetEase(_spinEase)
                .OnComplete(() => {
                    _isSpinning = false;
                    MessageBroker.Default.Publish(new OnWheelSpinEndEvent());
                })
                .SetId("WheelSpin");
        }
        
        private void OnDestroy()
        {
            DOTween.Kill("WheelSpin");
            _disposables?.Dispose();
        } 
        
        #if UNITY_EDITOR
        [ContextMenu("🎯 Test Spin")]
        private void TestSpin()
        {
            if (Application.isPlaying)
            {
                Debug.Log($"Test spin: {360f * _minRotations + Random.Range(0, _slotCount) * (360f / _slotCount)} degrees");
                StartSpin();
            }
        }
        
        [ContextMenu("🔄 Reset Rotation")]
        private void ResetRotation()
        {
            if (_wheelTransform != null)
            {
                DOTween.Kill("WheelSpin");
                _wheelTransform.rotation = Quaternion.identity;
                _isSpinning = false;
            }
        }
        #endif
    }
}