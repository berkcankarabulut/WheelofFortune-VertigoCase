using _Project.Scripts.Event.Storage;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Interaction
{
    // Oyun içerisinde persistentstorage panelini açma veya kapama amacı ile butonuna tıklamasını bildiriyo.
    public class PersistentStorageButtonInteraction : MonoBehaviour
    {
        [SerializeField] private Button _persistentStorageButton;

        private void Start()
        {
            if (_persistentStorageButton != null)
            {
                _persistentStorageButton.onClick.AddListener(HandlePersistentStorageButtonClick);
            }
        }

        private void HandlePersistentStorageButtonClick()
        {
            MessageBroker.Default.Publish(new OnShowPersistentStorageRequested());
        }

        private void OnDestroy()
        {
            if (_persistentStorageButton != null)
            {
                _persistentStorageButton.onClick.RemoveListener(HandlePersistentStorageButtonClick);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_persistentStorageButton == null)
            {
                _persistentStorageButton = GetComponent<Button>();
            }
        }
#endif
    }
}