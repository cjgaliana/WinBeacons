using System.Collections.Generic;
using System.Collections.ObjectModel;
using XBeacons.Core.Models;

namespace XamlBeacons.Services
{
    public interface IBeaconManager
    {
        ObservableCollection<XBeacon> Beacons { get; }

        void ProcessBeacons(IList<XBeacon> beacons);
    }
}