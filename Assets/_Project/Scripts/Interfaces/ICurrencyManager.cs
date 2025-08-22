namespace _Project.Scripts.Interfaces
{ 
    public interface ICurrencyManager
    { 
        int GetMoney();
        bool SpendCurrency(int amount);
    } 
}