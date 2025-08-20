using _Project.Scripts.Data.Wheel;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Service;
using _Project.Scripts.Utils;
using Zenject;

namespace _Project.Scripts.UI.Interaction
{
    public class ExitButtonInteracter : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;

        // DI injection
        private IWheelDataService _wheelDataService;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(IWheelDataService wheelDataService)
        {
            _wheelDataService = wheelDataService;
        }

        private void Start()
        {
            if (_exitButton == null) return; 
            if (_wheelDataService == null) return; 

            _exitButton.onClick.AddListener(HandleExitButtonClick);
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnWheelSpinStartEvent>()
                .Subscribe(_ => SetButtonInteractable(false))
                .AddTo(_disposables);

            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);
        }

        private void OnZoneChanged(OnZoneChangedEvent zone)
        {
            if (_wheelDataService == null) return;

            var wheelConfig = _wheelDataService.GetConfigsForZone(zone.CurrentZone);
            if (wheelConfig?.VisualConfig == null) return;

            bool isBronzeZone = wheelConfig.VisualConfig.Type == WheelType.BronzeZone;
            SetButtonInteractable(!isBronzeZone);
        }

        private void SetButtonInteractable(bool interactable)
        {
            if (_exitButton != null)
                _exitButton.interactable = interactable;
        }

        private void HandleExitButtonClick()
        {
            MessageBroker.Default.Publish(new OnExitRequestedEvent(true));
        }

        private void OnDestroy()
        {
            if (_exitButton != null)
            {
                _exitButton.onClick.RemoveListener(HandleExitButtonClick);
            }

            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_exitButton == null)
            {
                _exitButton = GetComponent<Button>();
            }
        }
#endif
    }
}