using _Project.Scripts.Config;
using _Project.Scripts.Data.Wheel; 

namespace _Project.Scripts.Service
{
    public class WheelDataService : IWheelDataService
    {
        private readonly WheelDatabaseSO _wheelDatabase;
        private readonly IGameSettings _gameSettings;

        // Constructor injection - GameSettingsSO inject edilecek
        public WheelDataService(WheelDatabaseSO wheelDatabase, IGameSettings gameSettings)
        {
            _wheelDatabase = wheelDatabase;
            _gameSettings = gameSettings;
        }

        public bool IsZoneSilver(int zone)
        {
            return zone % _gameSettings.SafeZoneInterval == 0;
        }

        public bool IsZoneGolden(int zone)
        {
            return zone % _gameSettings.SuperZoneInterval == 0;
        }

        public WheelType GetZoneType(int zone)
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