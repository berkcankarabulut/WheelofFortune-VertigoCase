using UnityEngine;

namespace _Project.Scripts.Data.Wheel
{
    [CreateAssetMenu(fileName = "WheelVisualConfig", menuName = "Project/Wheel/WheelVisualConfig")]
    public class WheelVisualConfig : ScriptableObject
    {
        [Header("Visual Settings")]
        [SerializeField] WheelType _wheelType;
        [SerializeField] private Sprite _wheelBackground;
        [SerializeField] private Sprite _wheelIndicator;
        [SerializeField] private string _wheelName;
        [SerializeField] private Color _wheelTitleColor;
        [SerializeField] private Sprite _zoneBackground;
        [SerializeField] private Color _zoneDisplayColor;

        public Sprite WheelBackground => _wheelBackground;

        public Sprite WheelIndicator => _wheelIndicator;

        public string WheelName => _wheelName;

        public WheelType Type => _wheelType;

        public Sprite ZoneBackground => _zoneBackground;

        public Color ZoneDisplayColor => _zoneDisplayColor;
        
        public Color WheelTitleColor => _wheelTitleColor;
    }
}