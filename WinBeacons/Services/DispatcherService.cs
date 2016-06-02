using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace WinBeacons.Services
{
    public class DispatcherService : IDispatcherService
    {
        public async Task RunAsync(Action action)
        {
            try
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        action();
                    });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}