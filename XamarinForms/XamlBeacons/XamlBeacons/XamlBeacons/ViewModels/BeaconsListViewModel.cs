using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using XamlBeacons.Services;
using XBeacons.Core.Interfaces;
using XBeacons.Core.Models;

namespace XamlBeacons.ViewModels
{
    public class BeaconsListViewModel : BaseViewModel
    {
        private readonly IBeaconManager _beaconManager;
        private readonly IBeaconWatcher _beaconWatcher;
        private readonly INavigationService _navigationService;

        private BeaconWatcherStatus _watcherStatus;

        public BeaconsListViewModel(INavigationService navigationService, IBeaconWatcher beaconWatcher,
            IBeaconManager beaconManager)
        {
            _navigationService = navigationService;
            _beaconWatcher = beaconWatcher;
            _beaconManager = beaconManager;

            CreateCommands();

            SubscribeToWatcherEvents();
        }

        public ObservableCollection<XBeacon> Beacons => _beaconManager.Beacons;

        public ICommand StartWatcherCommand { get; private set; }
        public ICommand StopWatcherCommand { get; private set; }

        public BeaconWatcherStatus WatcherStatus
        {
            get { return _watcherStatus; }
            set { Set(() => WatcherStatus, ref _watcherStatus, value); }
        }

        private void SubscribeToWatcherEvents()
        {
            _beaconWatcher.SignalReceived += OnBeaconSignalReceived;
            _beaconWatcher.StatusChanged += OnWatcherStatusChanged;
        }

        private void OnBeaconSignalReceived(object sender, BeaconSignalReceivedEventArgs e)
        {
            _beaconManager.ProcessBeacons(e.Beacons);
        }

        private void OnWatcherStatusChanged(object sender, BeaconWatcherStatusEventArgs e)
        {
            WatcherStatus = e.NewStatus;
        }

        private void CreateCommands()
        {
            StartWatcherCommand = new RelayCommand(StartWatcher);
            StopWatcherCommand = new RelayCommand(StopWatcher);
        }

        private void StartWatcher()
        {
            _beaconWatcher.Start();
        }

        private void StopWatcher()
        {
            _beaconWatcher.Stop();
        }
    }
}