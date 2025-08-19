using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Service;
using _Project.Scripts.UI.Wheel;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine; 

namespace _Project.Scripts.Runtime.Wheel
{
    public class WheelController : MonoBehaviour
    { 
        [SerializeField] private WheelVisualController wheelVisualController;
        [SerializeField] private WheelRewardSetter _wheelRewardSetter;
        private CompositeDisposable _disposables = new CompositeDisposable(); 

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
            WheelVisualConfig visualConfig = WheelDataService.Instance.GetConfigsForZone(zone).VisualConfig;   
            wheelVisualController.RefreshUI(
                visualConfig.WheelBackground,
                visualConfig.WheelIndicator,
                visualConfig.WheelName
            );
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

    }
}