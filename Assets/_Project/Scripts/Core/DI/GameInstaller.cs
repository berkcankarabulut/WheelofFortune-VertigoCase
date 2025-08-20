using Zenject;
using UnityEngine;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.Runtime.Wheel; 
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Manager;
using _Project.Scripts.Runtime.Zone; 

namespace _Project.Scripts.Core.DI
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Core Managers")] 
        [SerializeField] private CurrencyManager _currencyManager; 
        [SerializeField] private ZoneManager _zoneManager;
        [SerializeField] private MultiplierCalculator _multiplierCalculator;  
        
        [Header("Storage Systems")]
        [SerializeField] private CacheItemStorage _cacheItemStorage;
        [SerializeField] private PersistentItemStorage _persistentItemStorage;

        public override void InstallBindings()
        { 
            BindIfValid(_persistentItemStorage, () => 
                Container.Bind<PersistentItemStorage>().FromInstance(_persistentItemStorage).AsSingle()); 
            BindIfValid(_cacheItemStorage, () => 
                Container.Bind<CacheItemStorage>().FromInstance(_cacheItemStorage).AsSingle()); 
            BindIfValid(_currencyManager, () => 
                Container.Bind<ICurrencyManager>().FromInstance(_currencyManager).AsSingle());
            BindIfValid(_zoneManager, () => 
                Container.Bind<IZoneManager>().FromInstance(_zoneManager).AsSingle());   
            BindIfValid(_multiplierCalculator, () => 
                Container.Bind<MultiplierCalculator>().FromInstance(_multiplierCalculator).AsSingle()); 
        }

        private void BindIfValid<T>(T component, System.Action bindAction) where T : Object
        {
            if (component != null) 
                bindAction(); 
        }

#if UNITY_EDITOR
        private void OnValidate()
        { 
            _currencyManager ??= FindObjectOfType<CurrencyManager>();
            _zoneManager ??= FindObjectOfType<ZoneManager>();
            _multiplierCalculator ??= FindObjectOfType<MultiplierCalculator>();
             
            _cacheItemStorage ??= FindObjectOfType<CacheItemStorage>();
            _persistentItemStorage ??= FindObjectOfType<PersistentItemStorage>();  
        }
#endif
    }
}