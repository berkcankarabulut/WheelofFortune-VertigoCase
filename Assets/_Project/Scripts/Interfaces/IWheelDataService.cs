
using _Project.Scripts.Data.Reward;
using _Project.Scripts.Data.Wheel; 

namespace _Project.Scripts.Interfaces
{ 
    public interface IWheelDataService
    {
        WheelDataSO GetConfigsForZone(int zone);
        WheelType GetZoneType(int zone);
        bool IsZoneSilver(int zone);
        bool IsZoneGolden(int zone);
        RewardData GetBombReward();
    }
}