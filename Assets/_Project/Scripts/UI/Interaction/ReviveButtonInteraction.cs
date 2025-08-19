using _Project.Scripts.Event.Game; 
using UniRx;
using UnityEngine;
using UnityEngine.UI; 

namespace _Project.Scripts.UI.Interaction
{
    public class ReviveButtonInteraction : MonoBehaviour
    {
        [SerializeField] private Button _reviveButton;

        private void Start()
        {
            _reviveButton.onClick.AddListener(HandleReviveButtonClick); 
        }

        private void HandleReviveButtonClick()
        {  
            MessageBroker.Default.Publish(new OnReviveRequestedEvent());
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