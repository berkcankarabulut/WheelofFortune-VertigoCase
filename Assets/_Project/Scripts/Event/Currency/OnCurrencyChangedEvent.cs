using _Project.Scripts.Data.Item; 

namespace _Project.Scripts.Event.Currency
{
    public struct OnCurrencyChangedEvent
    {
        public ItemSO CurrencyItem { get; }
        public int PreviousAmount { get; }
        public int NewAmount { get; }
        public bool IsIncrease { get; }
        
        public OnCurrencyChangedEvent(ItemSO currencyItem, int previousAmount, int newAmount)
        {
            CurrencyItem = currencyItem;
            PreviousAmount = previousAmount;
            NewAmount = newAmount;
            IsIncrease = newAmount > previousAmount;
        }
    }
}