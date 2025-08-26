using UnityEngine;
using Zenject;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Data.Wheel;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Config;

namespace _Project.Scripts.DI
{
    // Burada Database'leri tutmamın amacı projedede hep sabit kalacak data türleri olması. Singleton mantığı ile kullanılıyorum burada. 
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [Header("Global Databases")]
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        
        [Header("Data Assets")] 
        [SerializeField] private WheelDatabaseSO _wheelDatabase;

        public override void InstallBindings()
        {
            BindIfValid(_itemDatabase, () =>
                Container.Bind<ItemDatabaseSO>().FromInstance(_itemDatabase).AsSingle());
            BindIfValid(_wheelDatabase, () =>
                Container.Bind<WheelDatabaseSO>().FromInstance(_wheelDatabase).AsSingle());
            Container.Bind<IWheelDataService>().To<WheelDataService>().AsSingle();
        }

        private void BindIfValid<T>(T component, System.Action bindAction) where T : Object
        {
            if (component != null)bindAction();    
        }
    }
}