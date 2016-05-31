using AltBeaconOrg.BoundBeacon;
using System;
using System.Collections.Generic;
using JavaObject = Java.Lang.Object;

namespace XBeacons.Droid
{
    public class RangeEventArgs : EventArgs
    {
        public Region Region { get; set; }
        public ICollection<Beacon> Beacons { get; set; }
    }

    public class RangeNotifier : JavaObject, IRangeNotifier
    {
        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            OnDidRangeBeaconsInRegion(beacons, region);
        }

        public event EventHandler<RangeEventArgs> DidRangeBeaconsInRegionComplete;

        private void OnDidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            var handler = DidRangeBeaconsInRegionComplete;
            if (handler != null)
            {
                handler.Invoke(this, new RangeEventArgs
                {
                    Beacons = beacons,
                    Region = region
                });
            }
        }
    }
}