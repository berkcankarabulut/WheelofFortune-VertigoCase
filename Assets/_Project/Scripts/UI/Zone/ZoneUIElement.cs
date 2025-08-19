using UnityEngine; 
using TMPro;  

namespace _Project.Scripts.UI.Wheel
{
    public class ZoneUIElement : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _zoneText_value; 
        
        public void UpdateDisplay(int zone, Color zoneColor)
        { 
            _zoneText_value.text = zone.ToString();
            _zoneText_value.color = zoneColor;
        } 
         
    }
}