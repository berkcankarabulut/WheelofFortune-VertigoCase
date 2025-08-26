using _Project.Scripts.Runtime.Zone;
using _Project.Scripts.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Zone
{
    // Wheel'ın altındaki her zone'da kazanç katlanıyor. Kaç kazanç katlanacağını yazar.
    public class MultiplierDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _multiplier_value;
        [SerializeField] private string _prefix = "x";
        [SerializeField] private string _suffix = "";

        private MultiplierCalculator _multiplierCalculator;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(MultiplierCalculator multiplierCalculator)
        {
            _multiplierCalculator = multiplierCalculator;
        }

        private void Start()
        {
            if (_multiplierCalculator == null) return;

            _multiplierCalculator.CurrentMultiplier
                .Subscribe(OnMultiplierChanged)
                .AddTo(_disposables);
        }

        private void OnMultiplierChanged(float multiplier)
        {
            UpdateDisplay(multiplier);
        }

        private void UpdateDisplay(float multiplier)
        {
            if (_multiplier_value == null) return; 
            string formattedMultiplier = NumberFormatter.FormatMultiplier(multiplier, 1);
            _multiplier_value.text = $"{_prefix} x{formattedMultiplier} {_suffix}";
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}