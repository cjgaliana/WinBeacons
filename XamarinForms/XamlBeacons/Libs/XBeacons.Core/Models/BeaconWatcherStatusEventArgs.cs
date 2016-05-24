namespace XBeacons.Core.Models
{
    public class BeaconWatcherStatusEventArgs
    {
        public BeaconWatcherStatus NewStatus { get; set; }
        public BeaconWatcherStatus OldStatus { get; set; }
    }
}