using UnityEngine;
using Zenject;
using _Project.Scripts.Config; 

namespace _Project.Scripts.Core.DI
{ 
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [Header("Required Settings")]
        [SerializeField] private GameSettingsSO _gameSettingsSO;

        public override void InstallBindings()
        { 
            Container.Bind<IGameSettings>()
                .FromInstance(_gameSettingsSO)
                .AsSingle();
        }  
    }
}