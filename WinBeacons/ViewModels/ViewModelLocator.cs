using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using WinBeacons.Services;

namespace WinBeacons.ViewModels
{
    public class ViewModelLocator : BaseViewModel
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();
            RegisterViewModels();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();

        private void RegisterServices()
        {
            SimpleIoc.Default.Register<IDispatcherService, DispatcherService>();
            SimpleIoc.Default.Register<INavigationService, CustomNavigationService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IBeaconService, BeaconService>();
            SimpleIoc.Default.Register<IStorageService, StorageService>();
        }

        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();
        }
    }
}