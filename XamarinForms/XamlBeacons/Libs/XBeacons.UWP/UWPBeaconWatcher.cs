using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.Advertisement;
using XBeacons.Core.Interfaces;
using XBeacons.Core.Models;

namespace XBeacons.UWP
{
    public class UWPBeaconWatcher : IBeaconWatcher
    {
        private readonly BluetoothLEAdvertisementWatcher _watcher;
        private BeaconWatcherStatus _status;

        public UWPBeaconWatcher()
        {
            // Create the Bluetooth LE watcher from the Windows 10 UWP
            _watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            _watcher.Received += OnWatcherReceived;
            _watcher.Stopped += OnWatcherStopped;
        }

        public event EventHandler<BeaconSignalReceivedEventArgs> SignalReceived;

        public event EventHandler<BeaconWatcherStatusEventArgs> StatusChanged;

        public BeaconWatcherStatus Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                {
                    return;
                }

                var oldValue = _status;
                _status = value;
                OnStatusChanged(_status, oldValue);
            }
        }

        public void Start()
        {
            _watcher.Start();
            RefreshStatus();
        }

        public void Stop()
        {
            if (_watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
            {
                _watcher.Stop();
            }
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            Status = GetStatus();
        }

        private async void OnWatcherReceived(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var handler = SignalReceived;
            if (handler != null)
            {
                var xbeaconsArgs = new BeaconSignalReceivedEventArgs
                {
                    Beacons = ExtractBeaconInfo(args)
                };
                handler.Invoke(this, xbeaconsArgs);
            }
        }

        private void OnWatcherStopped(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            RefreshStatus();
        }

        private void OnStatusChanged(BeaconWatcherStatus newValue, BeaconWatcherStatus oldValue)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                var args = new BeaconWatcherStatusEventArgs
                {
                    NewStatus = newValue,
                    OldStatus = oldValue
                };
                handler.Invoke(this, args);
            }
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

        private IList<XBeacon> ExtractBeaconInfo(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            return new List<XBeacon>();
        }
    }
}