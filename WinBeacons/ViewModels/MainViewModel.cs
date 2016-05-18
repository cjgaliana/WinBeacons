using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WinBeacons.Services;
using Windows.Devices.Bluetooth.Advertisement;

namespace WinBeacons.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IBeaconService _beaconService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IStorageService _storageService;
        private readonly IDispatcherService _dispatcherService;
        private ObservableCollection<string> _testData;

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

            TestData = new ObservableCollection<string>();
        }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public BeaconWatcherStatus WatcherStatus
        {
            get { return _watcherStatus; }
            set { Set(() => WatcherStatus, ref _watcherStatus, value); }
        }

        public ObservableCollection<string> TestData
        {
            get { return _testData; }
            set { Set(() => TestData, ref _testData, value); }
        }

        private void CreateCommands()
        {
            StartCommand = new RelayCommand(StartListening);
            StopCommand = new RelayCommand(StopListening);
        }

        private void RegisterEvents()
        {
            _beaconService.SignalReceived += OnBeaconSignalReceived;
            _beaconService.StatusChanged += OnStatusChanged;
        }

        private async void OnStatusChanged(object sender, BeaconWatcherStatus newStatus)
        {
            await this._dispatcherService.RunAsync(() =>
            {
                WatcherStatus = newStatus;
            });
        }

        private async void OnBeaconSignalReceived(object sender, BluetoothLEAdvertisementReceivedEventArgs e)
        {
            await this._dispatcherService.RunAsync(async () =>
             {
                 var sb = new StringBuilder();

                 sb.AppendLine($"Timestamp: {e.Timestamp}");
                 sb.AppendLine($"BT address: {e.BluetoothAddress}");
                 sb.AppendLine($"Type: {e.AdvertisementType}");
                 sb.AppendLine($"Signal Strength: {e.RawSignalStrengthInDBm} DBm");

                 TestData.Insert(0, sb.ToString());

                 await _storageService.TryAddItemAsync(e);
             }
          );
        }

        private void StartListening()
        {
            _beaconService.Start();
        }

        private void StopListening()
        {
            _beaconService.Stop();
        }
    }
}