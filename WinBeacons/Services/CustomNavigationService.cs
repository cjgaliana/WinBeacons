using GalaSoft.MvvmLight.Views;
using WinBeacons.Services;
using WinBeacons.Views;

namespace WinBeacons.Services
{
    public enum NavigationKeys
    {
        MainPage
    }

    public class CustomNavigationService : NavigationService
    {
        public CustomNavigationService()
        {
            this.Configure(NavigationKeys.MainPage.ToString(), typeof(MainPage));
        }
    }
}