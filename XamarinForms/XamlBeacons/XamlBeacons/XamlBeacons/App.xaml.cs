using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using XamlBeacons.Services;
using XamlBeacons.Views;

namespace XamlBeacons
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // The root page of your application
            var navService = ServiceLocator.Current.GetInstance<INavigationService>() as NavigationService;
            if (navService != null)
            {
                var rootPage = new NavigationPage(new BeaconsListView());
                MainPage = rootPage;
                navService.InitializeFrame(rootPage);
            }
        }

        public new static App Current => (App)Application.Current;

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}