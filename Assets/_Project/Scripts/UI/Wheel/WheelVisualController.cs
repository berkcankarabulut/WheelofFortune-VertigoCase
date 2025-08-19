using _Project.Scripts.Utils;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Wheel
{
    public class WheelVisualController : MonoBehaviour
    {
        [Header("Wheel Visuals")]
        [SerializeField] private Image _wheelImage;
        [SerializeField] private Image _wheelIndicator;
        [SerializeField] private TextMeshProUGUI _wheelTitleText;
     
        public void RefreshUI(Sprite wheelSprite, Sprite wheelIndicatorSprite, string wheelTitle, Color textColor)
        {
            if (_wheelImage != null)
                _wheelImage.sprite = wheelSprite;
                
            if (_wheelIndicator != null)
                _wheelIndicator.sprite = wheelIndicatorSprite;

            if (_wheelTitleText != null)
            {
                _wheelTitleText.text = wheelTitle;
                _wheelTitleText.color = textColor;
            } 
        }  
    }
}