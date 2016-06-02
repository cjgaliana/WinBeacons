using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using UniversalBeaconLibrary.Beacon;
using WinBeacons.Services;

namespace WinBeacons.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IBeaconService _beaconService;
        private readonly IDialogService _dialogService;
        private readonly IDispatcherService _dispatcherService;
        private readonly INavigationService _navigationService;
        private readonly IStorageService _storageService;
        private List<Beacon> _filteredBeacons;
        private Beacon.BeaconTypeEnum _selectedTypeForFilter;

        private BeaconWatcherStatus _watcherStatus;

        public MainViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IBeaconService beaconService,
            IStorageService storageService,
            IDispatcherService dispatcherService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _beaconService = beaconService;
            _storageService = storageService;
            _dispatcherService = dispatcherService;

            RegisterEvents();

            CreateCommands();

            // Init filter
            SelectedTypeForFilter = BeaconTypes.FirstOrDefault(x => x == Beacon.BeaconTypeEnum.iBeacon);

            // Refresh filters 
            this.Beacons.CollectionChanged += (s, e) =>
            {
                this.FilterBeacons();
            };
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ObservableCollection<Beacon> Beacons => _beaconService.Beacons;

        public List<Beacon.BeaconTypeEnum> BeaconTypes
            => Enum.GetValues(typeof(Beacon.BeaconTypeEnum)).Cast<Beacon.BeaconTypeEnum>().ToList();

        public BeaconWatcherStatus WatcherStatus
        {
            get { return _watcherStatus; }
            set { Set(() => WatcherStatus, ref _watcherStatus, value); }
        }

        public bool HasBeaconsInRage => Beacons.Count > 0;

        public Beacon.BeaconTypeEnum SelectedTypeForFilter
        {
            get { return _selectedTypeForFilter; }
            set
            {
                _selectedTypeForFilter = value;
                FilterBeacons();
            }
        }

        public List<Beacon> FilteredBeacons
        {
            get { return _filteredBeacons; }
            set { Set(() => FilteredBeacons, ref _filteredBeacons, value); }
        }

        private void CreateCommands()
        {
            StartCommand = new RelayCommand(StartListening);
            StopCommand = new RelayCommand(StopListening);
        }

        private void RegisterEvents()
        {
            //_beaconService.SignalReceived += OnBeaconSignalReceived;
            _beaconService.StatusChanged += OnStatusChanged;
            _beaconService.NewBeaconAdded += OnNewBeaconAdded;
        }

        private void OnNewBeaconAdded(object sender, Beacon e)
        {
            RaisePropertyChanged(() => HasBeaconsInRage);
        }

        private async void OnStatusChanged(object sender, BeaconWatcherStatus newStatus)
        {
            await _dispatcherService.RunAsync(() =>
            {
                WatcherStatus = newStatus;
            });
        }

        private void StartListening()
        {
            _beaconService.Start();
        }

        private void StopListening()
        {
            _beaconService.Stop();
        }

        private void FilterBeacons()
        {
            // Apply filters
            FilteredBeacons = Beacons
                .Where(x => x.BeaconType == SelectedTypeForFilter)
                .ToList();
        }
    }
}