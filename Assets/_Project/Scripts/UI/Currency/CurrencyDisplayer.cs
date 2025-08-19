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
        [SerializeField] private Image _currencyImage;
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private ItemSO _currencyItem;
        [SerializeField] private string _prefix = "";
        [SerializeField] private string _suffix = "";
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private bool _useAnimation = true;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private int _currentDisplayValue = 0;

        private void Start()
        {
            AdressableAtlasManager.LoadSprite(_currencyItem.Icon, _currencyImage);
            UpdateText(_currentDisplayValue);
            MessageBroker.Default.Receive<OnCurrencyChangedEvent>()
                .Where(e => _currencyItem != null && e.CurrencyItem.Equals(_currencyItem.Id.ToGuid()))
                .Subscribe(OnCurrencyChanged)
                .AddTo(_disposables);
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