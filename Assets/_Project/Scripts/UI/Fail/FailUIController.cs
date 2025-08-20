using System;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.UI;
using _Project.Scripts.Event.Game;
using _Project.Scripts.UI.Interaction; 

namespace _Project.Scripts.UI.Fail
{
    public class FailUIController : MonoBehaviour
    {
        [Header("UI Components")]  
        [SerializeField] private GameObject _failPanel; 
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Animation _failPanelAnimation;
        [Header("Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.OutBack;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private Vector3 _originalPosition;
        private Vector3 _originalScale;

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
                .Subscribe(_ => HideFailUI())
                .AddTo(_disposables);
        }

        private void ShowFailUI(OnGameFailedEvent onGameFailedEvent)
        {  
            _reviveButton.interactable = onGameFailedEvent.CanRevive;
            _failPanel.SetActive(true);  
            _failPanelAnimation.Play();
        }

        private void HideFailUI()
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
            if (_reviveButton == null)
            {
                ReviveButtonInteraction reviveButton = GetComponentInChildren<ReviveButtonInteraction>(true);
                if (reviveButton != null)
                    _reviveButton = reviveButton.GetComponent<Button>();
            }
        }
#endif
    }
}