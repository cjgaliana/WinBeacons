﻿// Copyright 2015 Andreas Jakl, Tieto Corporation. All rights reserved.
// https://github.com/andijakl/universal-beacon
//
// Based on the Google Eddystone specification,
// available under Apache License, Version 2.0 from
// https://github.com/google/eddystone
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;

namespace UniversalBeaconLibrary.Beacon
{
    /// <summary>
    ///     Represents a single unique beacon that has a specified Bluetooth MAC address.
    ///     Construction and updates are usually handled by the BeaconManager.
    ///     Construct this class based on a Bluetooth advertisement received from the
    ///     Windows Bluetooth API. When further advertisements are received for this beacon,
    ///     call its UpdateBeacon() method to update the frames.
    /// </summary>
    public class Beacon : INotifyPropertyChanged
    {
        public enum BeaconTypeEnum
        {
            /// <summary>
            ///     Bluetooth LE advertisment that is not recognized as one of the beacon formats
            ///     supported by this library.
            /// </summary>
            Unknown,

            /// <summary>
            ///     Beacon conforming to the Google Eddystone specification.
            /// </summary>
            Eddystone,

            /// <summary>
            ///     Beacon conforming to the Apple iBeacon specification.
            ///     iBeacon is a Trademark of Apple Inc.
            ///     Note: the beacon broadcast payload is not parsed by this library.
            /// </summary>
            iBeacon
        }

        private readonly Guid _eddystoneGuid = new Guid("0000FEAA-0000-1000-8000-00805F9B34FB");

        private ulong _bluetoothAddress;


        private short _rssi;

        private DateTimeOffset _timestamp;

        /// <summary>
        ///     Construct a new Bluetooth beacon based on the received advertisement.
        ///     Tries to find out if it's a known type, and then parses the contents accordingly.
        /// </summary>
        /// <param name="btAdv">
        ///     Bluetooth advertisement to parse, as received from
        ///     the Windows Bluetooth LE API.
        /// </param>
        public Beacon(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            BluetoothAddress = btAdv.BluetoothAddress;
            UpdateBeacon(btAdv);
        }

        /// <summary>
        ///     Manually create a new Beacon instance.
        /// </summary>
        /// <param name="beaconType">Beacon type to use for this manually constructed beacon.</param>
        public Beacon(BeaconTypeEnum beaconType)
        {
            BeaconType = beaconType;
        }

        /// <summary>
        ///     Type of this beacon.
        ///     Defines how the beacon will parse the individual frames to extract information from the
        ///     advertisements.
        /// </summary>
        public BeaconTypeEnum BeaconType { get; set; } = BeaconTypeEnum.Unknown;

        /// <summary>
        ///     List of all the different frames that have been observed for this beacon so far.
        ///     If a new frame with the same type is collected, it replaces the previous frame.
        /// </summary>
        public ObservableCollection<BeaconFrameBase> BeaconFrames { get; set; } =
            new ObservableCollection<BeaconFrameBase>();

        public double DistanceEstimation => GetDistanceToBeacon();

        /// <summary>
        ///     Raw signal strength in dBM.
        ///     If a new advertisement is received for the same beacon (with the same
        ///     Bluetooth MAC address), always the latest signal strength is recorded.
        /// </summary>
        public short Rssi
        {
            get { return _rssi; }
            set
            {
                if (_rssi == value) return;
                _rssi = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     The Bluetooth MAC address.
        ///     Used to cluster the different received Bluetooth advertisements and to
        ///     collect multiple advertisements for unique beacons.
        /// </summary>
        public ulong BluetoothAddress
        {
            get { return _bluetoothAddress; }
            set
            {
                if (_bluetoothAddress == value) return;
                _bluetoothAddress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BluetoothAddressAsString));
            }
        }

        /// <summary>
        ///     Retrieves the Bluetooth MAC address formatted as a hex string.
        /// </summary>
        public string BluetoothAddressAsString
        {
            get
            {
                return
                    string.Join(":", BitConverter.GetBytes(BluetoothAddress).Reverse().Select(b => b.ToString("X2")))
                        .Substring(6);
            }
        }

        /// <summary>
        ///     Timestamp when the last advertisement was received for this beacon.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp == value) return;
                _timestamp = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private double GetDistanceToBeacon()
        {
            if (BeaconFrames != null)
            {
                var requiredFrame =
                    BeaconFrames.FirstOrDefault(x => x.GetType().Name == nameof(ProximityBeaconFrame));

                if (requiredFrame != null)
                {
                    var proximityFrame = requiredFrame as ProximityBeaconFrame;
                    if (proximityFrame != null)
                    {
                        var tx = proximityFrame.TxPower;
                        var rssi = Rssi;


                        var dist = CalculateDistance(tx, rssi);
                        return dist;
                    }
                }
            }

            return -1;
        }


        protected double CalculateDistance(int txPower, double rssi)
        {
            //extracted from http://developer.radiusnetworks.com/2014/12/04/fundamentals-of-beacon-ranging.html

            var TOLERANCE = 0.0000001;
            if (Math.Abs(rssi) < TOLERANCE)
            {
                return -1.0; // if we cannot determine distance, return -1.
            }

            var ratio = rssi*1.0/txPower;
            if (ratio < 1.0)
            {
                return Math.Pow(ratio, 10);
            }
            var accuracy = 0.89976*Math.Pow(ratio, 7.7095) + 0.111;
            return accuracy;
        }

        /// <summary>
        ///     Received a new advertisement for this beacon.
        ///     If the Bluetooth address of the new advertisement matches this beacon,
        ///     it will parse the contents of the advertisement and extract known frames.
        /// </summary>
        /// <param name="btAdv">
        ///     Bluetooth advertisement to parse, as received from
        ///     the Windows Bluetooth LE API.
        /// </param>
        public void UpdateBeacon(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            if (btAdv == null) return;

            if (btAdv.BluetoothAddress != BluetoothAddress)
            {
                throw new BeaconException("Bluetooth address of beacon does not match - not updating beacon information");
            }

            Rssi = btAdv.RawSignalStrengthInDBm;
            Timestamp = btAdv.Timestamp;

            //Debug.WriteLine($"Beacon advertisment detected (Strength: {Rssi}): Address: {BluetoothAddress}");

            // Check if beacon advertisement contains any actual usable data
            if (btAdv.Advertisement == null) return;

            if (btAdv.Advertisement.ServiceUuids.Any())
            {
                foreach (var serviceUuid in btAdv.Advertisement.ServiceUuids)
                {
                    // If we have multiple service UUIDs and already recognized a beacon type,
                    // don't overwrite it with another service Uuid.
                    if (BeaconType == BeaconTypeEnum.Unknown)
                    {
                        BeaconType = serviceUuid.Equals(_eddystoneGuid)
                            ? BeaconTypeEnum.Eddystone
                            : BeaconTypeEnum.Unknown;
                    }
                }
            }

            // Data sections
            if (btAdv.Advertisement.DataSections.Any())
            {
                if (BeaconType == BeaconTypeEnum.Eddystone)
                {
                    // This beacon is according to the Eddystone specification - parse data
                    ParseEddystoneData(btAdv);
                }
                else if (BeaconType == BeaconTypeEnum.Unknown)
                {
                    // Unknown beacon type
                    //Debug.WriteLine("\nUnknown beacon");
                    //foreach (var dataSection in btAdv.Advertisement.DataSections)
                    //{
                    //    Debug.WriteLine("Data section 0x: " + dataSection.DataType.ToString("X") + " = " +
                    //        BitConverter.ToString(dataSection.Data.ToArray()));
                    //}
                }
            }

            // Manufacturer data - currently unused
            if (btAdv.Advertisement.ManufacturerData.Any())
            {
                foreach (var manufacturerData in btAdv.Advertisement.ManufacturerData)
                {
                    // Print the company ID + the raw data in hex format
                    //var manufacturerDataString = $"0x{manufacturerData.CompanyId.ToString("X")}: {BitConverter.ToString(manufacturerData.Data.ToArray())}";
                    //Debug.WriteLine("Manufacturer data: " + manufacturerDataString);

                    var manufacturerDataArry = manufacturerData.Data.ToArray();
                    if (BeaconFrameHelper.IsProximityBeaconPayload(manufacturerData.CompanyId, manufacturerDataArry))
                    {
                        BeaconType = BeaconTypeEnum.iBeacon;
                        //Debug.WriteLine("iBeacon Frame: " + BitConverter.ToString(manufacturerDataArry));

                        var beaconFrame = new ProximityBeaconFrame(manufacturerDataArry);

                        // Only one relevant data frame for iBeacons
                        if (BeaconFrames.Any())
                        {
                            BeaconFrames[0].Update(beaconFrame);
                        }
                        else
                        {
                            BeaconFrames.Add(beaconFrame);
                        }
                        OnPropertyChanged(nameof(DistanceEstimation));
                    }
                }
            }
        }

        private void ParseEddystoneData(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            // Parse Eddystone data
            foreach (var dataSection in btAdv.Advertisement.DataSections)
            {
                //Debug.WriteLine("Beacon data: " + dataSection.DataType + " = " +
                //                BitConverter.ToString(dataSection.Data.ToArray()));
                //+ " (" + Encoding.UTF8.GetString(dataSection.Data.ToArray()) + ")\n");

                // Relvant data of Eddystone is in data section 0x16
                // Windows receives: 0x01 = 0x06
                //                   0x03 = 0xAA 0xFE
                //                   0x16 = 0xAA 0xFE [type] [data]
                if (dataSection.DataType == 0x16)
                {
                    var beaconFrame = dataSection.Data.ToArray().CreateEddystoneBeaconFrame();
                    if (beaconFrame == null) continue;

                    var found = false;

                    for (var i = 0; i < BeaconFrames.Count; i++)
                    {
                        if (BeaconFrames[i].GetType() == beaconFrame.GetType())
                        {
                            var updateFrame = false;
                            if (beaconFrame.GetType() == typeof(UnknownBeaconFrame))
                            {
                                // Unknown frame - also compare eddystone type
                                var existingEddystoneFrameType =
                                    BeaconFrames[i].Payload.GetEddystoneFrameType();
                                var newEddystoneFrameType = beaconFrame.Payload.GetEddystoneFrameType();
                                if (existingEddystoneFrameType != null &&
                                    existingEddystoneFrameType == newEddystoneFrameType)
                                {
                                    updateFrame = true;
                                }
                            }
                            else
                            {
                                updateFrame = true;
                            }
                            if (updateFrame)
                            {
                                BeaconFrames[i].Update(beaconFrame);
                                found = true;
                                break; // Don't analyze any other known frames of this beacon
                            }
                        }
                    }
                    if (!found)
                    {
                        BeaconFrames.Add(beaconFrame);
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}