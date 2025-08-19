using Zenject;
using UnityEngine;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.Runtime.Wheel;
using _Project.Scripts.Service;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Manager;

namespace _Project.Scripts.Core.DI
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Core Managers")] 
        [SerializeField] private CurrencyManager _currencyManager; 
        [SerializeField] private ZoneManager _zoneManager;
        
        [Header("Storage Systems")]
        [SerializeField] private CacheItemStorage _cacheItemStorage;
        [SerializeField] private PersistentItemStorage _persistentItemStorage;
        
        [Header("Wheel Core System")] 
        [SerializeField] private WheelSpinner _wheelSpinner;
        [SerializeField] private WheelRewardSetter _wheelRewardSetter;
        
        [Header("Data Assets")] 
        [SerializeField] private WheelDatabaseSO _wheelDatabase;

        public override void InstallBindings()
        { 
            BindIfValid(_wheelDatabase, () => 
                Container.Bind<WheelDatabaseSO>().FromInstance(_wheelDatabase).AsSingle()); 
            Container.Bind<IWheelDataService>().To<WheelDataService>().AsSingle(); 
            BindIfValid(_persistentItemStorage, () => 
                Container.Bind<PersistentItemStorage>().FromInstance(_persistentItemStorage).AsSingle());
            BindIfValid(_cacheItemStorage, () => 
                Container.Bind<CacheItemStorage>().FromInstance(_cacheItemStorage).AsSingle()); 
            BindIfValid(_currencyManager, () => 
                Container.Bind<ICurrencyManager>().FromInstance(_currencyManager).AsSingle());
            BindIfValid(_zoneManager, () => 
                Container.Bind<IZoneManager>().FromInstance(_zoneManager).AsSingle()); 
            BindIfValid(_wheelSpinner, () => 
                Container.Bind<WheelSpinner>().FromInstance(_wheelSpinner).AsSingle());
            BindIfValid(_wheelRewardSetter, () => 
                Container.Bind<WheelRewardSetter>().FromInstance(_wheelRewardSetter).AsSingle());
        }

        private void BindIfValid<T>(T component, System.Action bindAction) where T : Object
        {
            if (component != null) 
                bindAction();
            else 
                Debug.LogError($"[GameInstaller] {typeof(T).Name} is not assigned!");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Core Managers
            _currencyManager ??= FindObjectOfType<CurrencyManager>();
            _zoneManager ??= FindObjectOfType<ZoneManager>();
            
            // Storage Systems
            _cacheItemStorage ??= FindObjectOfType<CacheItemStorage>();
            _persistentItemStorage ??= FindObjectOfType<PersistentItemStorage>();
            
            // Wheel Core System
            _wheelSpinner ??= FindObjectOfType<WheelSpinner>();
            _wheelRewardSetter ??= FindObjectOfType<WheelRewardSetter>(); 
        }
#endif
    }
}