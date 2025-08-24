using _Project.Scripts.Data.Item;
using UnityEngine;
using TMPro;
using UniRx;
using _Project.Scripts.Event.Currency;
using _Project.Scripts.Utils;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Currency
{
    public class CurrencyDisplayer : MonoBehaviour
    {
        [SerializeField] private RewardItemSO currencyRewardItem;
        [SerializeField] private Image _currencyImage;
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private string _prefix = "";
        [SerializeField] private string _suffix = "";
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private bool _useAnimation = true;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private int _currentDisplayValue = 0;

        private void Awake()
        {
            MessageBroker.Default.Receive<OnCurrencyChangedEvent>()
                .Where(e => currencyRewardItem != null && e.CurrencyRewardItem != null &&
                            e.CurrencyRewardItem.Id.Equals(currencyRewardItem.Id))
                .Subscribe(OnCurrencyChanged)
                .AddTo(_disposables);
        }

        private void Start()
        {
            if (currencyRewardItem?.Icon != null)
            {
                AddressableAtlasLoader.LoadSprite(currencyRewardItem.Icon, _currencyImage);
            }


            UpdateText(_currentDisplayValue);
        }

        private void OnCurrencyChanged(OnCurrencyChangedEvent currencyEvent)
        { 
            if (_useAnimation)
            {
                NumberAnimator.AnimateNumber(_currentDisplayValue, currencyEvent.NewAmount, _animationDuration,
                    OnAnimationUpdate);
            }
            else
            {
                _currentDisplayValue = currencyEvent.NewAmount;
                UpdateText(_currentDisplayValue);
            }
        }

        private void OnAnimationUpdate(int value)
        {
            _currentDisplayValue = value;
            UpdateText(value);
        }

        private void UpdateText(int value)
        {
            if (_currencyText == null) return;
            string formattedValue = NumberFormatter.FormatDecimal(value);
            _currencyText.text = $"{_prefix}{formattedValue}{_suffix}";
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}