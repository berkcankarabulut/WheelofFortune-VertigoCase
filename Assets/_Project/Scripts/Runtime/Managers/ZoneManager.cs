using UnityEngine;
using UniRx;
using _Project.Scripts.Event.Zone;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Utils;

namespace _Project.Scripts.Runtime.Wheel
{
    public class ZoneManager : MonoBehaviour, IZoneManager
    {
        private int _currentZone = 1;

        public void NextZone()
        {
            _currentZone++;

            this.Log($"Moving to next zone: {_currentZone}");
            PublishZoneChanged(_currentZone);
        }

        private void PublishZoneChanged(int zone)
        {
            var zoneEvent = new OnZoneChangedEvent(zone);
            MessageBroker.Default.Publish(zoneEvent);
            this.Log($"Published OnZoneChangedEvent for zone: {zone}");
        }
    }
}