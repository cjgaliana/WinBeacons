using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using XBeacons.Core.Interfaces;
using XBeacons.Core.Models;

namespace XBeacons.Droid
{
    public class DroidBeaconWatcher : IBeaconWatcher
    {
        private readonly BeaconManager _beaconManager;

        private readonly Context _context;
        private readonly RangeNotifier _rangeNotifier;
        private Region _beaconRegion;

        private BeaconWatcherStatus _status = BeaconWatcherStatus.Unknow;

        public DroidBeaconWatcher()
        {
            _context = Application.Context;

            VerityBluetooth();

            _beaconManager = BeaconManager.GetInstanceForApplication(_context);
            _rangeNotifier = new RangeNotifier();

            _rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;
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
            Status = BeaconWatcherStatus.Started;

            _beaconManager.
        }

        public void Stop()
        {
            Status = BeaconWatcherStatus.Stopped;
        }

        private void RangingBeaconsInRegion(object sender, RangeEventArgs e)
        {
            var allBeacons = new List<Beacon>();
            if (e.Beacons.Count > 0)
            {
                foreach (var b in e.Beacons)
                {
                    allBeacons.Add(b);
                }

                var orderedBeacons = allBeacons.OrderBy(b => b.Distance).ToList();
                UpdateData(orderedBeacons);
            }
            else
            {
                // unknown
            }
        }

        private void UpdateData(List<Beacon> beacons)
        {
            //var newBeacons = new List<Beacon>();
            //foreach (var beacon in beacons)
            //{
            //    if (_data.All(b => b.Id1.ToString() == beacon.Id1.ToString()))
            //    {
            //        newBeacons.Add(beacon);
            //    }
            //}

            //RunOnUiThread(() =>
            //{
            //    foreach (var beacon in newBeacons)
            //    {
            //        _data.Add(beacon);
            //    }

            //    if (newBeacons.Count > 0)
            //    {
            //        _data.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            //        UpdateList();
            //    }
            //});
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

        private void VerityBluetooth()
        {
            try
            {
                if (!BeaconManager.GetInstanceForApplication(_context).CheckAvailability())
                {
                    var builder = new AlertDialog.Builder(_context);
                    builder.SetTitle("Bluetooth not enabled");
                    builder.SetMessage("Please enable bluetooth in settings and restart this application.");
                    EventHandler<DialogClickEventArgs> handler = null;
                    builder.SetPositiveButton(Android.Resource.String.Ok, handler);
                    builder.Show();
                }
            }
            catch (BleNotAvailableException e)
            {
                Log.Debug("BleNotAvailableException", e.Message);

                var builder = new AlertDialog.Builder(_context);
                builder.SetTitle("Bluetooth LE not available");
                builder.SetMessage("Sorry, this device does not support Bluetooth LE.");
                EventHandler<DialogClickEventArgs> handler = null;
                builder.SetPositiveButton(Android.Resource.String.Ok, handler);
                builder.Show();
            }
        }
    }
}