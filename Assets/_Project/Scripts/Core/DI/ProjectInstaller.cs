using UnityEngine;
using Zenject;
using _Project.Scripts.Config;
using _Project.Scripts.Data.Reward;

namespace _Project.Scripts.Core.DI
{ 
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [Header("Global Settings")]
        [SerializeField] private GameSettingsSO _gameSettingsSO;
        
        [Header("Global Databases")]
        [SerializeField] private ItemDatabaseSO _itemDatabase;

        public override void InstallBindings()
        {   
            BindIfValid(_gameSettingsSO, () => 
                Container.Bind<IGameSettings>().FromInstance(_gameSettingsSO).AsSingle()); 
            BindIfValid(_itemDatabase, () => 
                Container.Bind<ItemDatabaseSO>().FromInstance(_itemDatabase).AsSingle()); 
        }  
        
        private void BindIfValid<T>(T component, System.Action bindAction) where T : Object
        {
            if (component != null) 
                bindAction();
            else 
                Debug.LogError($"[ProjectInstaller] {typeof(T).Name} is not assigned!");
        }
    }
}