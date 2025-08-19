using System.Linq;
using _Project.Scripts.Data.Item;
using UnityEngine;
using UniRx;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Currency;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.Utils;

namespace _Project.Scripts.Runtime.Managers
{
    public class CurrencyManager : MonoBehaviour
    {
        [SerializeField] private PersistentItemStorage _persistentStorage;
        [SerializeField] private ItemSO _currencyItem;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private ReactiveProperty<int> _currentMoney = new ReactiveProperty<int>(0);

        // Public reactive property - dışarıdan subscribe edilebilir
        public IReadOnlyReactiveProperty<int> CurrentMoney => _currentMoney;

        private void OnEnable()
        {
            if (_persistentStorage != null)
                _persistentStorage.OnLoaded += PublishCurrencyEvent;

            MessageBroker.Default.Receive<OnCurrencyRequestEvent>()
                .Subscribe(OnCurrencyRequested)
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            if (_persistentStorage != null)
                _persistentStorage.OnLoaded -= PublishCurrencyEvent;
        }

        private void OnCurrencyRequested(OnCurrencyRequestEvent requestEvent)
        {
            bool success = SpendMoney(requestEvent.Amount);

            var responseEvent = new OnCurrencyWithdrawEvent(
                _currencyItem,
                requestEvent.Amount,
                success ? requestEvent.Amount : 0,
                success,
                success ? "Success" : "Insufficient funds"
            );

            MessageBroker.Default.Publish(responseEvent);
        }

        public int GetMoney()
        {
            if (_currencyItem == null || _persistentStorage == null) return 0;

            return _persistentStorage.GetAll()
                .Where(reward => reward.ItemSo != null && reward.ItemSo.Id.Equals(_currencyItem.Id.ToGuid()))
                .Sum(reward => reward.Amount);
        }

        public bool SpendMoney(int amount)
        {
            if (amount <= 0 || GetMoney() < amount) return false;

            var currencyRewards = _persistentStorage.GetAll()
                .Where(reward => reward.ItemSo != null && reward.ItemSo.Equals(_currencyItem.Id.ToGuid()))
                .ToList();

            int toRemove = amount;
            foreach (var reward in currencyRewards)
            {
                if (toRemove <= 0) break;

                if (reward.Amount <= toRemove)
                {
                    toRemove -= reward.Amount;
                    _persistentStorage.Remove(reward);
                }
                else
                {
                    _persistentStorage.Remove(reward);
                    _persistentStorage.Add(new ItemAmountData(reward.ItemSo, reward.Amount - toRemove));
                    toRemove = 0;
                }
            }

            UpdateCurrentMoney();
            return true;
        }

        private void UpdateCurrentMoney()
        {
            int newAmount = GetMoney();
            _currentMoney.Value = newAmount;
            PublishCurrencyChanged(newAmount);
        }

        private void PublishCurrencyEvent()
        {
            _persistentStorage.OnLoaded -= PublishCurrencyEvent;
            UpdateCurrentMoney();
        }

        private void PublishCurrencyChanged(int currentAmount)
        {
            MessageBroker.Default.Publish(new OnCurrencyChangedEvent(_currencyItem, currentAmount, currentAmount));
        }

        private void OnDestroy()
        {
            _currentMoney?.Dispose();
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        [ContextMenu("Log Money")]
        private void LogMoney() => this.Log($"Current Money: {GetMoney()}");
#endif
    }
}