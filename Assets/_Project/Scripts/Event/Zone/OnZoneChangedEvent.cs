namespace _Project.Scripts.Event.Zone
{
    public struct OnZoneChangedEvent
    {
        public int CurrentZone { get; }
        public OnZoneChangedEvent(int currentZone)
        {
            CurrentZone = currentZone;
        }
    }
}