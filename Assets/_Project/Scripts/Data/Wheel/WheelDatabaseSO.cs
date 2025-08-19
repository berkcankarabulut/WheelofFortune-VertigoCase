using System.Linq; 
using UnityEngine;

namespace _Project.Scripts.Data.Wheel
{ 
    [CreateAssetMenu(fileName = "WheelDatabaseSO", menuName = "Project/Wheel/WheelDatabaseSO")]
    public class WheelDatabaseSO : ScriptableObject
    {
        [SerializeField] private WheelDataSO[] _wheels;

        public WheelDataSO GetByType(WheelType type)
        {
            return _wheels.FirstOrDefault(x => x.Type == type);
        }
    }
}