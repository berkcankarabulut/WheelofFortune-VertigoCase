using UnityEngine;
using UniRx;
using DG.Tweening;
using _Project.Scripts.Event.Game;
using _Project.Scripts.UI.Interaction;
using _Project.Scripts.Utils;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Fail
{
    public class FailUIController : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private GameObject _failPanel;

        [SerializeField] private Button _reviveButton;

        [Header("Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;

        [SerializeField] private Ease _showEase = Ease.OutBack;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnGameFailedEvent>()
                .Subscribe(ShowFailUI)
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnRevivedEvent>()
                .Subscribe(DisableFailUI)
                .AddTo(_disposables);
        }

        private void ShowFailUI(OnGameFailedEvent onGameFailedEvent)
        { 
            if (_failPanel == null) return;
            _reviveButton.interactable = onGameFailedEvent.CanRevive;

            _failPanel.SetActive(true);

            _failPanel.transform.localScale = Vector3.zero;

            _failPanel.transform.DOScale(Vector3.one, _showDuration)
                .SetEase(_showEase);
        }

        private void DisableFailUI(OnRevivedEvent onRevivedEvent)
        {
            _failPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(_reviveButton != null) return;
            ReviveButtonInteraction reviveButton = GetComponentInChildren<ReviveButtonInteraction>(true);
            if (reviveButton == null) return;
            _reviveButton = reviveButton.GetComponent<Button>();
        }
#endif
    }
}