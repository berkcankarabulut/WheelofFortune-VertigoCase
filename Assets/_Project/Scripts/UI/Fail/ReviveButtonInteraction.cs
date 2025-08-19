using _Project.Scripts.Event.Game; 
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Fail
{
    public class ReviveButtonInteraction : MonoBehaviour
    {
        [SerializeField] private Button _reviveButton; 
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            InitializeEventSubscriptions();
        }

        private void InitializeEventSubscriptions()
        {
            MessageBroker.Default.Receive<OnGameFailedEvent>()
                .Subscribe(SetButtonInteractable)
                .AddTo(_disposables);
        } 
        
        private void SetButtonInteractable(OnGameFailedEvent gameFailed)
        {
            if (_reviveButton != null)
                _reviveButton.interactable = gameFailed.CanRevive; 
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_reviveButton == null)
            {
                _reviveButton = GetComponent<Button>();
            }
        }
#endif
    }
}