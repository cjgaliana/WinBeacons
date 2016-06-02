using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using UniversalBeaconLibrary.Beacon;

namespace WinBeacons.Services
{
    public class BeaconService : IBeaconService
    {
        private readonly BeaconManager _beaconManager;
        private readonly IDispatcherService _dispatcherService;
        private readonly BluetoothLEAdvertisementWatcher _watcher;

        public BeaconService(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;

            _beaconManager = new BeaconManager();

            // Create the Bluetooth LE watcher from the Windows 10 UWP
            _watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            OnStatusChanged();
        }

        public BeaconWatcherStatus Status => GetStatus();

        public ObservableCollection<Beacon> Beacons
            => _beaconManager?.BluetoothBeacons ?? new ObservableCollection<Beacon>();

        public event EventHandler<Beacon> NewBeaconAdded;

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

        private void OnBeaconAdded(Beacon newBeacon)
        {
            var handler = NewBeaconAdded;
            handler?.Invoke(this, newBeacon);
        }

        private async Task ProcessBeacon(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            await _dispatcherService.RunAsync(() =>
            {
                var before = _beaconManager.BluetoothBeacons.Count;
                _beaconManager.ReceivedAdvertisement(args);
                var after = _beaconManager.BluetoothBeacons.Count;

                if (before < after)
                {
                    var beacon =
                        _beaconManager.BluetoothBeacons.FirstOrDefault(
                            x => x.BluetoothAddress == args.BluetoothAddress);

                    if (beacon != null)
                    {
                        OnBeaconAdded(beacon);
                    }
                }
            });
        }
    }
}