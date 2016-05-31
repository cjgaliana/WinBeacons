using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using XamlBeacons.Services;
using XBeacons.Core.Interfaces;

namespace XamlBeacons.ViewModels
{
    public class ViewModelLocator
    {
        private readonly UnityContainer _container;

        public ViewModelLocator()
        {
            _container = new UnityContainer();

            var locator = new UnityServiceLocator(_container);
            ServiceLocator.SetLocatorProvider(() => locator);

            RegisterServices();
            RegisterViewModels();
        }

        public BeaconsListViewModel BeaconsListViewModel => _container.Resolve<BeaconsListViewModel>();
        public BeaconDetailsViewModel BeaconDetailsViewModel => _container.Resolve<BeaconDetailsViewModel>();

        private void RegisterServices()
        {
            _container
                .RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager())
                .RegisterType<IBeaconManager, BeaconManager>(new ContainerControlledLifetimeManager());


            var nativeBeaconService = DependencyService.Get<IBeaconWatcher>();
            _container.RegisterInstance(nativeBeaconService);
        }

        private void RegisterViewModels()
        {
            _container
                .RegisterType<BeaconsListViewModel>(new ContainerControlledLifetimeManager())
                .RegisterType<BeaconDetailsViewModel>();
        }
    }
}