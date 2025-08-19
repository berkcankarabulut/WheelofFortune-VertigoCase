namespace _Project.Scripts.Config
{ 
    public interface IGameSettings
    {
        int SafeZoneInterval { get; }
        int SuperZoneInterval { get; }
        int RevivePrice { get; }
        float ZoneRewardMultiplier { get; }
        float SafeRewardMultiplier { get; }
        float SuperZoneMultiplier { get; }
    }
}