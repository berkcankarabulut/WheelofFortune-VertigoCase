using UniRx;

namespace _Project.Scripts.Interfaces
{ 
    public interface ICurrencyManager
    {
        IReadOnlyReactiveProperty<int> CurrentMoney { get; }
        int GetMoney();
        bool SpendMoney(int amount);
    } 
}