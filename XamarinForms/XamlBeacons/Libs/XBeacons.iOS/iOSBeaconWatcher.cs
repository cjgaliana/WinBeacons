using System;
using System.Collections.Generic;
using CoreLocation;
using Foundation;
using XBeacons.Core.Interfaces;
using XBeacons.Core.Models;

namespace XBeacons.iOS
{
    public class iOSBeaconWatcher : IBeaconWatcher
    {
        private readonly CLLocationManager _locator;
        private readonly CLBeaconRegion _beaconRegion;

        private BeaconWatcherStatus _status = BeaconWatcherStatus.Unknow;

        public iOSBeaconWatcher()
        {
            var uuid = new NSUuid("A1F30FF0-0A9F-4DE0-90DA-95F88164942E");
            var beaconID = "iOSBeacon";
            _beaconRegion = new CLBeaconRegion(uuid, beaconID)
            {
                NotifyEntryStateOnDisplay = true,
                NotifyOnEntry = true,
                NotifyOnExit = true
            };

            _locator = new CLLocationManager();
            //_locator.RegionEntered += RegionEntered;
            _locator.DidRangeBeacons += DidRangeBeacons;
        }

        public event EventHandler<BeaconSignalReceivedEventArgs> SignalReceived;

        public event EventHandler<BeaconWatcherStatusEventArgs> StatusChanged;

        public BeaconWatcherStatus Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                {
                    return;
                }

                var oldValue = _status;
                _status = value;
                OnStatusChanged(_status, oldValue);
            }
        }

        public void Start()
        {
            _locator.StartMonitoring(_beaconRegion);
            Status = BeaconWatcherStatus.Started;
        }

        public void Stop()
        {
            _locator.StopMonitoring(_beaconRegion);
            Status = BeaconWatcherStatus.Stopped;
        }

        private void DidRangeBeacons(object sender, CLRegionBeaconsRangedEventArgs e)
        {
            if (e.Beacons != null && e.Beacons.Length > 0)
            {
                var handler = SignalReceived;
                if (handler != null)
                {
                    var beacons = ExtractBeaconsInfo(e.Beacons);
                    var args = new BeaconSignalReceivedEventArgs
                    {
                        Beacons = beacons
                    };
                    handler.Invoke(this, args);
                }
            }
        }

        private IList<XBeacon> ExtractBeaconsInfo(CLBeacon[] beacons)
        {
            foreach (var clBeacon in beacons)
            {
                var xbeacon = new XBeacon
                {
                    EstimatedDistanceInMeters = clBeacon.Accuracy,
                    Rssi = Convert.ToInt32(clBeacon.Rssi),
                    Major = clBeacon.Major.ToString(),
                    Minor = clBeacon.Minor.ToString(),
                    Guid = clBeacon.ProximityUuid.ToString()
                };

                switch (clBeacon.Proximity)
                {
                    case CLProximity.Immediate:
                        xbeacon.Proximity = XBeaconProximity.Immediate;
                        break;

                    case CLProximity.Near:
                        xbeacon.Proximity = XBeaconProximity.Near;
                        break;

                    case CLProximity.Far:
                        xbeacon.Proximity = XBeaconProximity.Far;
                        break;

                    case CLProximity.Unknown:
                    default:
                        xbeacon.Proximity = XBeaconProximity.Unknown;
                        break;
                }
            }

            return new List<XBeacon>();
        }

        private void OnStatusChanged(BeaconWatcherStatus newValue, BeaconWatcherStatus oldValue)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                var args = new BeaconWatcherStatusEventArgs
                {
                    NewStatus = newValue,
                    OldStatus = oldValue
                };
                handler.Invoke(this, args);
            }
        }
    }
}