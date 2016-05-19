using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UniversalBeaconLibrary.Beacon;
using Windows.Devices.Bluetooth.Advertisement;

namespace WinBeacons.Services
{
    public enum BeaconWatcherStatus
    {
        Unknow,
        Started,
        Stopped,
        Created,
        Stopping,
        Aborted
    }

    public interface IBeaconService
    {
        BeaconWatcherStatus Status { get; }

        ObservableCollection<Beacon> Beacons { get; }

        event EventHandler<BluetoothLEAdvertisementReceivedEventArgs> SignalReceived;

        event EventHandler<BeaconWatcherStatus> StatusChanged;

        void Start();

        void Stop();
    }

    public class BeaconService : IBeaconService
    {
        private readonly IDispatcherService _dispatcherService;
        private readonly BeaconManager _beaconManager;
        private readonly BluetoothLEAdvertisementWatcher _watcher;

        public BeaconService(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
            _beaconManager = new BeaconManager();
            Beacons = _beaconManager.BluetoothBeacons;
            // Create the Bluetooth LE watcher from the Windows 10 UWP
            _watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            OnStatusChanged();
        }

        public BeaconWatcherStatus Status => GetStatus();
        public ObservableCollection<Beacon> Beacons { get; }

        public event EventHandler<BluetoothLEAdvertisementReceivedEventArgs> SignalReceived;

        public event EventHandler<BeaconWatcherStatus> StatusChanged;

        public void Start()
        {
            _watcher.Received += OnWatcherReceived;
            _watcher.Stopped += OnWatcherStopped;

            _watcher.Start();
            OnStatusChanged();
        }

        public void Stop()
        {
            if (_watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
            {
                _watcher.Stop();
            }
        }

        private void OnWatcherStopped(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            OnStatusChanged();
        }

        private BeaconWatcherStatus GetStatus()
        {
            if (_watcher == null)
            {
                return BeaconWatcherStatus.Unknow;
            }

            switch (_watcher.Status)
            {
                case BluetoothLEAdvertisementWatcherStatus.Created:
                    return BeaconWatcherStatus.Created;

                case BluetoothLEAdvertisementWatcherStatus.Started:
                    return BeaconWatcherStatus.Started;

                case BluetoothLEAdvertisementWatcherStatus.Stopping:
                    return BeaconWatcherStatus.Stopping;

                case BluetoothLEAdvertisementWatcherStatus.Stopped:
                    return BeaconWatcherStatus.Stopped;

                case BluetoothLEAdvertisementWatcherStatus.Aborted:
                    return BeaconWatcherStatus.Aborted;

                default:
                    return BeaconWatcherStatus.Unknow;
            }
        }

        private async void OnWatcherReceived(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            // Handle event in the beacon service
            await ProcessBeacon(args);

            // Propagate event
            var handler = SignalReceived;
            handler?.Invoke(sender, args);
        }

        private void OnStatusChanged()
        {
            var handler = StatusChanged;
            handler?.Invoke(this, Status);
        }

        private async Task ProcessBeacon(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            await this._dispatcherService.RunAsync(() =>
            {
                _beaconManager.ReceivedAdvertisement(args);
            });
        }
    }
}