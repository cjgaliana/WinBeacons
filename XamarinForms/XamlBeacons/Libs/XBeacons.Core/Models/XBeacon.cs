namespace XBeacons.Core.Models
{
    public class XBeacon
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public double EstimatedDistanceInMeters { get; set; }
        public XBeaconProximity Proximity { get; set; }
        public int Rssi { get; set; }
    }
}