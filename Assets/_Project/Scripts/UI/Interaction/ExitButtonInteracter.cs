using _Project.Scripts.Config;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Wheel;
using _Project.Scripts.Event.Zone; 

namespace _Project.Scripts.UI.Interaction
{
    public class ExitButtonInteracter : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
 
        private CompositeDisposable _disposables = new CompositeDisposable();
 

        private void Start()
        {
            if (_exitButton == null) return;  
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
             if(zone.CurrentZone % GameSettings.SAFE_ZONE_INTERVAL == 0 || zone.CurrentZone % GameSettings.SUPER_ZONE_INTERVAL == 0)
                 SetButtonInteractable(true);
             else SetButtonInteractable(false);
           
        }

        private void SetButtonInteractable(bool interactable)
        {
            if (_exitButton != null)
                _exitButton.interactable = interactable;
        }

        private void HandleExitButtonClick()
        {
            MessageBroker.Default.Publish(new OnSafeExitRequestedEvent(true));
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