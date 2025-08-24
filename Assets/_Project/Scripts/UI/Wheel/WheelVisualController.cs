using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Wheel
{
    public class WheelVisualController : MonoBehaviour
    {
        [Header("Wheel Visuals")]
        [SerializeField] private Image _wheelImage;
        [SerializeField] private Image _wheelIndicator;
        [SerializeField] private TextMeshProUGUI _wheelTitleText;
        [Inject] private IWheelDataService _wheelDataService; 
        private CompositeDisposable _disposables = new CompositeDisposable();  
        private void Awake()
        { 
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);  
        }
        
        public void OnZoneChanged(OnZoneChangedEvent zoneChangedEvent)
        {
            WheelVisualConfig visualConfig = _wheelDataService.GetConfigsForZone(zoneChangedEvent.CurrentZone).VisualConfig;   
            RefreshUI(
                visualConfig.WheelBackground,
                visualConfig.WheelIndicator,
                visualConfig.WheelName,
                visualConfig.WheelTitleColor
            );        }
        
        public void RefreshUI(Sprite wheelSprite, Sprite wheelIndicatorSprite, string wheelTitle, Color textColor)
        {
            if (_wheelImage != null)
                _wheelImage.sprite = wheelSprite;
                
            if (_wheelIndicator != null)
                _wheelIndicator.sprite = wheelIndicatorSprite;

            if (_wheelTitleText != null)
            {
                _wheelTitleText.text = wheelTitle;
                _wheelTitleText.color = textColor;
            } 
        }  
    }
}