using System;
using System.Threading.Tasks;

namespace WinBeacons.Services
{
    public interface IDispatcherService
    {
        Task RunAsync(Action action);
    }
}