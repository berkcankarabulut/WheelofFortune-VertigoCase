using UnityEngine;
using UniRx;
using DG.Tweening;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Utils;

namespace _Project.Scripts.UI.Fail
{
    public class FailUIController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject _failPanel;
        
        [Header("Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            InitializeUI();
            InitializeEventSubscriptions();
        }

        private void InitializeUI()
        {
            if (_failPanel != null)
                _failPanel.SetActive(false);
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnGameFailedEvent>()
                .Subscribe(_ => ShowFailUI())
                .AddTo(_disposables);
        }

        private void ShowFailUI()
        {
            if (_failPanel == null) return;
            
            _failPanel.SetActive(true);
            _failPanel.transform.localScale = Vector3.zero;
            
            _failPanel.transform.DOScale(Vector3.one, _showDuration)
                .SetEase(_showEase)
                .OnComplete(() => this.Log("Fail UI shown"));
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}