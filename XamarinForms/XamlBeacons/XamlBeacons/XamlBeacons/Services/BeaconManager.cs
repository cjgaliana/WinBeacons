using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XamlBeacons.ViewModels;
using XBeacons.Core.Models;

namespace XamlBeacons.Services
{
    public class BeaconManager : BaseViewModel, IBeaconManager
    {
        public BeaconManager()
        {
        }

        public ObservableCollection<XBeacon> Beacons { get; set; } = new ObservableCollection<XBeacon>();

        public void ProcessBeacons(IList<XBeacon> beacons)
        {
            foreach (var xBeacon in beacons)
            {
                var existing = this.Beacons.FirstOrDefault(x => x.Guid == xBeacon.Guid);
                if (existing != null)
                {
                    // Update
                    existing.EstimatedDistanceInMeters = xBeacon.EstimatedDistanceInMeters;
                    existing.Major = xBeacon.Major;
                    existing.Minor = xBeacon.Minor;
                    existing.Name = xBeacon.Name;
                    existing.Proximity = xBeacon.Proximity;
                    existing.Rssi = xBeacon.Rssi;
                }
                else
                {
                    // Add
                    this.Beacons.Add(xBeacon);
                }
            }
        }
    }
}