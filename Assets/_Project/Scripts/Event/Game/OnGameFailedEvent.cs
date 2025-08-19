namespace _Project.Scripts.Event.Game
{
    public struct OnGameFailedEvent
    {
        public bool CanRevive { get; } 
        
        public OnGameFailedEvent(bool canRevive = true)
        {
            CanRevive = canRevive; 
        }
    }
}