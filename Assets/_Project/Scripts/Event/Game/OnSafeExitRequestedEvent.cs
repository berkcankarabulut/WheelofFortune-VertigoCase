namespace _Project.Scripts.Event.Game
{
    public struct OnSafeExitRequestedEvent
    {
        public bool ConfirmExit { get; }
        
        public OnSafeExitRequestedEvent(bool confirmExit = true)
        {
            ConfirmExit = confirmExit;
        }
    }
}