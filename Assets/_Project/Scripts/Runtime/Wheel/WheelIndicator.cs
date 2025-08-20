using DG.Tweening;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Event.Wheel;
using _Project.Scripts.Event.Reward;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelIndicator : MonoBehaviour
    {
        [SerializeField] private Collider2D _indicatorCollider;
        [SerializeField] private Transform _rewardDetectionPoint;
        [SerializeField] private float _raycastDistance = 2f;
        [SerializeField] private float _rotateAngle = 5f;
        [SerializeField] private float _rotateDuration = 0.1f;
        [SerializeField] private Ease _rotateEase = Ease.OutBack;

        private bool _isTweenEnabled = false;
        private Tween _rotateTween;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            _indicatorCollider.isTrigger = true;

            MessageBroker.Default.Receive<OnWheelSpinStartEvent>()
                .Subscribe(_ => _isTweenEnabled = true)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnWheelSpinEndEvent>()
                .Subscribe(_ => OnSpinCompleted())
                .AddTo(_disposables);
        }

        private void OnSpinCompleted()
        {
            WheelRewardUIElement reward = GetRewardWithRaycast();
            
            if (reward != null)
            {
                reward.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() => {
                        MessageBroker.Default.Publish(new OnTryCollectRewardEvent(reward.GetRewardData()));
                        DisableKick();
                    });
            }
            else
            {
                DisableKick();
            }
        }

        private WheelRewardUIElement GetRewardWithRaycast()
        {
            Vector2 startPos = _rewardDetectionPoint ? _rewardDetectionPoint.position : transform.position;
            Vector2 direction = _rewardDetectionPoint ? (_rewardDetectionPoint.position - transform.position).normalized : Vector2.down; 
            var hits = Physics2D.RaycastAll(startPos, direction, _raycastDistance); 
            foreach (var hit in hits)
            {
                if (hit.collider == _indicatorCollider) continue; 
                if (!hit.collider.CompareTag("WheelRewardUI")) continue;
                return hit.collider.GetComponent<WheelRewardUIElement>();
            } 
            return null;
        }

        private void DisableKick()
        {
            _isTweenEnabled = false;
            _rotateTween?.Kill();
            if (transform.parent) transform.rotation = Quaternion.identity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTweenEnabled && other.CompareTag("WheelRewardUI"))
                PerformKick();
        }

        private void PerformKick()
        {
            _rotateTween?.Kill();
            _rotateTween = DOTween.Sequence()
                .Append(transform.DORotate(new Vector3(0, 0, _rotateAngle), _rotateDuration))
                .Append(transform.DORotate(Vector3.zero, _rotateDuration))
                .SetEase(_rotateEase)
                .SetId("IndicatorKick")
                .OnComplete(() => { _isTweenEnabled = false;});
        }

        private void OnDestroy()
        {
            _rotateTween?.Kill();
            _disposables?.Dispose();
        }
    }
}