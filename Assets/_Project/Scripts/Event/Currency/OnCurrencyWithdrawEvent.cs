using _Project.Scripts.Data.Item; 

namespace _Project.Scripts.Event.Currency
{
    public struct OnCurrencyWithdrawEvent
    {
        public ItemSO CurrencyItem { get; }
        public int RequestedAmount { get; }
        public int ActualAmount { get; }
        public bool IsSuccessful { get; }
        public string Reason { get; }

        public OnCurrencyWithdrawEvent(ItemSO currencyItem, int requestedAmount, int actualAmount,
            bool isSuccessful, string reason = "")
        {
            CurrencyItem = currencyItem;
            RequestedAmount = requestedAmount;
            ActualAmount = actualAmount;
            IsSuccessful = isSuccessful;
            Reason = reason;
        }
 
        public OnCurrencyWithdrawEvent(ItemSO currencyItem, int amount) : this(currencyItem, amount, amount, true,
            "Success")
        {
        } 
        
        public OnCurrencyWithdrawEvent(ItemSO currencyItem, int requestedAmount, string failureReason) : this(
            currencyItem, requestedAmount, 0, false, failureReason)
        {
        }
    }
}