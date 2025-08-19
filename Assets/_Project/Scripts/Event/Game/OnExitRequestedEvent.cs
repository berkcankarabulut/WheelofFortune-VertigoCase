namespace _Project.Scripts.Event.Game
{
    public struct OnExitRequestedEvent
    {
        public bool ConfirmExit { get; }
        
        public OnExitRequestedEvent(bool confirmExit = true)
        {
            ConfirmExit = confirmExit;
        }
    }
}