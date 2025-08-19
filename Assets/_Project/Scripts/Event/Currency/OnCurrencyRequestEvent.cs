namespace _Project.Scripts.Event.Currency
{
    public struct OnCurrencyRequestEvent
    {
        public int Amount { get; }
        public string RequestId { get; }
        public string Source { get; }
        
        public OnCurrencyRequestEvent(int amount, string source = "", string requestId = "")
        {
            Amount = amount;
            Source = source;
            RequestId = string.IsNullOrEmpty(requestId) ? System.Guid.NewGuid().ToString() : requestId;
        }
    }
}