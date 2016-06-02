using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace WinBeacons.Services
{
    public class StorageService : IStorageService
    {
        private readonly AsyncLock _mutex = new AsyncLock();

        public async Task TryAddItemAsync(BluetoothLEAdvertisementReceivedEventArgs item)
        {
            try
            {
                using (await _mutex.LockAsync())
                {
                    var json = JsonConvert.SerializeObject(item);
                    var file = await GetFileAsync();

                    await FileIO.AppendTextAsync(file, json);
                }
            }
            catch (Exception ex)
            {
                var a = 5;
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