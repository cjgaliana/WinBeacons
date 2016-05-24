using System;
using XBeacons.Core.Models;

namespace XBeacons.Core.Interfaces
{
    public interface IBeaconWatcher
    {
        BeaconWatcherStatus Status { get; }

        event EventHandler<BeaconSignalReceivedEventArgs> SignalReceived;

        event EventHandler<BeaconWatcherStatusEventArgs> StatusChanged;

        void Start();

        void Stop();
    }
}