using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Service;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Utils; 
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelController : MonoBehaviour
    { 
        [SerializeField] private WheelVisualController wheelVisualController;
         
        private IWheelRewardSetter _wheelRewardSetter;
        private IWheelDataService _wheelDataService; 
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(
            WheelRewardSetter wheelRewardSetter,
            IWheelDataService wheelDataService)
        {
            _wheelRewardSetter = (IWheelRewardSetter)wheelRewardSetter;
            _wheelDataService = wheelDataService;
        }

        private void Awake()
        {
            InitializeEventSubscriptions();
        } 
        
        private void InitializeEventSubscriptions()
        { 
            MessageBroker.Default.Receive<OnZoneChangedEvent>()
                .Subscribe(OnZoneChanged)
                .AddTo(_disposables);  
            
            MessageBroker.Default.Receive<OnGameStartedEvent>()
                .Subscribe(Initialize)
                .AddTo(_disposables); 
            
            this.Log("Subscribed to OnZoneChangedEvent via MessageBroker");
        }

        private void Initialize(OnGameStartedEvent onGameStartedEvent)
        {
            WheelUpdate(0);
        }
        
        private void OnZoneChanged(OnZoneChangedEvent zoneEvent)
        { 
            WheelUpdate(zoneEvent.CurrentZone);
        }

        private void WheelUpdate(int zone)
        {
            LoadUI(zone);
            _wheelRewardSetter.LoadRewards(zone);
        }

        private void LoadUI(int zone)
        { 
            WheelVisualConfig visualConfig = _wheelDataService.GetConfigsForZone(zone).VisualConfig;   
            wheelVisualController.RefreshUI(
                visualConfig.WheelBackground,
                visualConfig.WheelIndicator,
                visualConfig.WheelName,
                visualConfig.WheelTitleColor
            );
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}