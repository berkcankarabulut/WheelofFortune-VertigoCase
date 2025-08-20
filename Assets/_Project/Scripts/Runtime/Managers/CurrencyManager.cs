using System.Linq;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Currency;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Storage;
using _Project.Scripts.Utils;
using UniRx;
using UnityEngine; 
using Zenject;

namespace _Project.Scripts.Runtime.Manager
{
    public class CurrencyManager : MonoBehaviour, ICurrencyManager
    {
        [SerializeField] private RewardItemSO currencyRewardItem;

        private PersistentItemStorage _persistentStorage;
        private ReactiveProperty<int> _currentMoney = new ReactiveProperty<int>(0);
        private CompositeDisposable _disposables = new CompositeDisposable();

        public IReadOnlyReactiveProperty<int> CurrentMoney => _currentMoney;

        [Inject]
        public void Construct(PersistentItemStorage persistentStorage) => _persistentStorage = persistentStorage;

        public int GetMoney() => GetCurrencyReward()?.Amount ?? 0;

        private void Awake()
        { 
            MessageBroker.Default.Receive<OnSaveLoadedEvent>()
                .Subscribe(Initialize)
                .AddTo(_disposables);
        }

        private void Start()
        {
            if (_persistentStorage == null || _persistentStorage.Count <= 0) return; 
            UpdateCurrentMoney();
        }

        private void Initialize(OnSaveLoadedEvent onSaveLoadedEvent)
        { 
            UpdateCurrentMoney();
        }

        public bool SpendMoney(int amount)
        {
            if (amount <= 0 || GetMoney() < amount) return false;

            int previousAmount = GetMoney();  
            var currency = GetCurrencyReward();
            if (currency == null) return false;

           bool result = _persistentStorage.Remove(currency);
           if (!result) return false;
            if (currency.Amount > amount)
                _persistentStorage.Add(new RewardData(currencyRewardItem, currency.Amount - amount));

            UpdateCurrentMoney(previousAmount);   
            return true;
        }

        public bool AddMoney(int amount)
        {
            if (amount <= 0 || currencyRewardItem == null) return false;

            int previousAmount = GetMoney(); 
            var existing = GetCurrencyReward();
            if (existing != null) _persistentStorage.Remove(existing);

            _persistentStorage.Add(new RewardData(currencyRewardItem, (existing?.Amount ?? 0) + amount));
            UpdateCurrentMoney(previousAmount);   
            return true;
        }

        private RewardData GetCurrencyReward()
        {
            if (_persistentStorage == null || currencyRewardItem == null) return null;
            
            return _persistentStorage.GetAll()
                .FirstOrDefault(r => r?.RewardItemSo != null && r.RewardItemSo.Id.Equals(currencyRewardItem.Id));
        }

        private void UpdateCurrentMoney(int previousAmount = -1)
        {
            int currentAmount = GetMoney(); 
            if (previousAmount == -1) 
                previousAmount = _currentMoney.Value;

            _currentMoney.Value = currentAmount; 
            MessageBroker.Default.Publish(new OnCurrencyChangedEvent(currencyRewardItem, previousAmount, currentAmount));
        }

        private void OnDestroy()
        {
            _currentMoney?.Dispose();
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        [ContextMenu("💰 Log Money")] 
        private void LogMoney() 
        {
            var currency = GetCurrencyReward();
            this.Log($"Money: {GetMoney()} | Currency Found: {currency != null} | Storage Items: {_persistentStorage?.Count ?? 0}");
        }
        
        [ContextMenu("➕ Add 100")] private void Add100() => AddMoney(100);
        [ContextMenu("➕ Add 500")] private void Add500() => AddMoney(500);
        [ContextMenu("➖ Spend 50")] private void Spend50() => SpendMoney(50);
        
        [ContextMenu("🔄 Force Update")] 
        private void ForceUpdate() => UpdateCurrentMoney();
        
        [ContextMenu("📊 Debug Storage")]
        private void DebugStorage()
        {
            if (_persistentStorage == null)
            {
                this.LogWarning("Storage is null!");
                return;
            }
            
            var items = _persistentStorage.GetAll();
            this.Log($"=== STORAGE DEBUG ===");
            this.Log($"Total items: {items.Count}");
            this.Log($"Currency Item ID: {currencyRewardItem?.Id}");
            
            foreach (var item in items)
            {
                this.Log($"Item: {item.RewardItemSo?.Name} (ID: {item.RewardItemSo?.Id}) - Amount: {item.Amount}");
                
                if (currencyRewardItem != null && item.RewardItemSo != null)
                {
                    bool isMatch = item.RewardItemSo.Id.Equals(currencyRewardItem.Id);
                    this.Log($"  → Matches currency: {isMatch}");
                }
            }
        }
#endif
    }
}