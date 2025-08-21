using System.Linq;
using _Project.Scripts.Config;
using _Project.Scripts.Data.Item;
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Event.Currency;
using _Project.Scripts.Event.Game;
using _Project.Scripts.Event.Save;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Runtime.Storage; 
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
             
            MessageBroker.Default.Receive<OnReviveRequestedEvent>()
                .Subscribe(OnReviveRequested)
                .AddTo(_disposables);
        }

        private void OnReviveRequested(OnReviveRequestedEvent reviveEvent)
        {
            bool spendSuccess = SpendMoney(GameSettings.REVIVE_PRICE);
            if (!spendSuccess) return;
 
            MessageBroker.Default.Publish(new OnPlayerRevivedEvent());
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
    }
}