using _Project.Scripts.Event.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Fail
{
    public class GiveUpButtonInteraction : MonoBehaviour
    {
        [SerializeField] private Button _giveUpButton;

        private void Awake()
        {
            _giveUpButton.onClick.AddListener(() =>
                MessageBroker.Default.Publish(new OnGameOveredEvent())
            );
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_giveUpButton == null)
            {
                _giveUpButton = GetComponent<Button>();
            }
        }
#endif
    }
}