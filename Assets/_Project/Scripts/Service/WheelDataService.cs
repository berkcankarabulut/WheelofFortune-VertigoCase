using _Project.Scripts.Config;
using _Project.Scripts.Utils;
using _Project.Scripts.Data.Wheel;
using UnityEngine;

namespace _Project.Scripts.Service
{
    public class WheelDataService : Singleton<WheelDataService>
    {
        [SerializeField] WheelDatabaseSO _wheelDatabase;

        private bool IsZoneSilver(int zone)
        {
            return zone % GameSettings.SafeZoneInterval == 0;
        }

        private bool IsZoneGolden(int zone)
        {
            return zone % GameSettings.SuperZoneInterval == 0;
        }

        private WheelType GetZoneType(int zone)
        {
            if (zone == 0) return WheelType.BronzeZone;

            if (IsZoneGolden(zone)) return WheelType.GoldenZone;
            return IsZoneSilver(zone) ? WheelType.SilverZone : WheelType.BronzeZone;
        }

        public WheelDataSO GetConfigsForZone(int zone)
        {
            WheelType wheelType = GetZoneType(zone);
            return _wheelDatabase.GetByType(wheelType);
        }
    }
}