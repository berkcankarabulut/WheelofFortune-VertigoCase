using UnityEngine;
using UniRx;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;

namespace _Project.Scripts.Runtime.Wheel
{
    public class ZoneManager : MonoBehaviour, IZoneManager
    {
        private int _currentZone = 1;

        public void NextZone()
        {
            _currentZone++;
            PublishZoneChanged(_currentZone);
        }

        private void PublishZoneChanged(int zone) => MessageBroker.Default.Publish(new OnZoneChangedEvent(zone));
    }
}