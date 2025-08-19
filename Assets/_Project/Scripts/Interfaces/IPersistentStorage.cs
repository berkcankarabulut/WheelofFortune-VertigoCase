namespace _Project.Scripts.Interfaces
{
    public interface IPersistentStorage : IItemStorage
    {
        System.Action OnLoaded { get; set; }
    }
}