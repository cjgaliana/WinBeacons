using System;
using System.Collections.ObjectModel;
using UniversalBeaconLibrary.Beacon;
using Windows.Devices.Bluetooth.Advertisement;

namespace WinBeacons.Services
{
    public interface IBeaconService
    {
        BeaconWatcherStatus Status { get; }

        ObservableCollection<Beacon> Beacons { get; }

        event EventHandler<Beacon> NewBeaconAdded;

        event EventHandler<BluetoothLEAdvertisementReceivedEventArgs> SignalReceived;

        event EventHandler<BeaconWatcherStatus> StatusChanged;

        void Start();

        void Stop();
    }
}