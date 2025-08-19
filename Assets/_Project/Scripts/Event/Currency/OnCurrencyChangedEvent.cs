using _Project.Scripts.Data.Item; 

namespace _Project.Scripts.Event.Currency
{
    public struct OnCurrencyChangedEvent
    {
        public RewardItemSO CurrencyRewardItem { get; }
        public int PreviousAmount { get; }
        public int NewAmount { get; }
        public bool IsIncrease { get; }
        
        public OnCurrencyChangedEvent(RewardItemSO currencyRewardItem, int previousAmount, int newAmount)
        {
            CurrencyRewardItem = currencyRewardItem;
            PreviousAmount = previousAmount;
            NewAmount = newAmount;
            IsIncrease = newAmount > previousAmount;
        }
    }
}