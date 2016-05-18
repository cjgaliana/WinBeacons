using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage;

namespace WinBeacons.Services
{
    public interface IStorageService
    {
        Task TryAddItemAsync(BluetoothLEAdvertisementReceivedEventArgs item);
    }

    public class StorageService : IStorageService
    {
        public async Task TryAddItemAsync(BluetoothLEAdvertisementReceivedEventArgs item)
        {
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var file = await GetFileAsync();
                await FileIO.AppendTextAsync(file, json);
            }
            catch (Exception)
            {
            }
        }

        public async Task<StorageFile> GetFileAsync()
        {
            var rootFolder = ApplicationData.Current.LocalFolder;
            var file = await rootFolder.CreateFileAsync("data", CreationCollisionOption.OpenIfExists);

            return file;
        }
    }
}