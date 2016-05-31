using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XBeacons.Core.Interfaces;
using XBeacons.Droid;

namespace XamlBeacons.Droid
{
    [Activity(Label = "XamlBeacons", Icon = "@drawable/icon", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            DependencyService.Register<IBeaconWatcher, DroidBeaconWatcher>();
            LoadApplication(new App());
        }
    }
}