using System.Collections.Generic;

namespace XBeacons.Core.Models
{
    public class BeaconSignalReceivedEventArgs
    {
        public IList<XBeacon> Beacons { get; set; }
    }
}