using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace WinBeacons.Services
{
    public interface IStorageService
    {
        Task TryAddItemAsync(BluetoothLEAdvertisementReceivedEventArgs item);
    }
}