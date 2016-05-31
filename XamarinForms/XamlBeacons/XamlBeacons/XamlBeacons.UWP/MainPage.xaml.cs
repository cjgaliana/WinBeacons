using Xamarin.Forms;
using XBeacons.Core.Interfaces;
using XBeacons.UWP;

namespace XamlBeacons.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            DependencyService.Register<IBeaconWatcher, UWPBeaconWatcher>();
            LoadApplication(new XamlBeacons.App());
        }
    }
}