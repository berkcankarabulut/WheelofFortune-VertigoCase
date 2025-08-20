using _Project.Scripts.Event.Game;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using _Project.Scripts.Event.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Utils;
using AssetKits.ParticleImage;

namespace _Project.Scripts.UI.Wheel
{
    public class WheelSpinButtonInteracter : MonoBehaviour
    {
        [SerializeField] private ParticleImage _buttonParticleImage;
        [SerializeField] private Button _spinButton;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.3f;

        [SerializeField] private float _disabledScale = 0.85f;
        [SerializeField] private Ease _scaleEase = Ease.OutBack;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private Vector3 _originalScale;
        private Tween _scaleTween;

        private void Start()
        {
            if (_spinButton == null)
            {
                this.LogError("Spin button is not assigned!");
                return;
            }

            _spinButton.onClick.AddListener(() =>
                MessageBroker.Default.Publish(new OnRequestWheelSpinStartEvent()));
            _originalScale = _spinButton.transform.localScale;

            MessageBroker.Default.Receive<OnWheelSpinStartEvent>()
                .Subscribe(_ => SetButtonInteractable(false))
                .AddTo(_disposables); 
            
            MessageBroker.Default.Receive<OnPlayerRevivedEvent>()
                .Subscribe(_ => SetButtonInteractable(true))
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(_ => SetButtonInteractable(true))
                .AddTo(_disposables);
        }

        private void SetButtonInteractable(bool interactable)
        {
            if (_spinButton == null) return;
            if (_buttonParticleImage != null)
                _buttonParticleImage.enabled = interactable;
            
            _spinButton.interactable = interactable;
            _scaleTween?.Kill();

            Vector3 targetScale = interactable ? _originalScale : _originalScale * _disabledScale;
            _scaleTween = _spinButton.transform.DOScale(targetScale, _animationDuration)
                .SetEase(_scaleEase)
                .SetId("SpinButtonScale");
        }

        private void OnDestroy()
        {
            _spinButton?.onClick.RemoveAllListeners();
            _scaleTween?.Kill();
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_spinButton == null)
                _spinButton = GetComponent<Button>();
        }
#endif
    }
}