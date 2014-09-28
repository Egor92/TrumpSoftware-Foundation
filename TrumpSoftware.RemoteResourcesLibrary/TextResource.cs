using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class TextResource : Resource
    {
        public TextResource(ResourceManager resourceManager, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo) 
            : base(resourceManager, localResourceInfo, remoteResourceInfo)
        {
        }

        protected override async Task<object> DownloadFromServer(Uri uri)
        {
            return await HttpClient.GetStringAsync(uri);
        }

        protected override async Task<object> LoadFromLocalStorage(StorageFolder storageFolder, string path)
        {
            var storageFile = await storageFolder.GetFileAsync(path);
            if (storageFile == null)
                return null;
            return await FileIO.ReadTextAsync(storageFile);
        }

        protected override async Task<object> LoadFromCompiledResource(string path)
        {
            var storageFile = await StorageFile.GetFileFromPathAsync(path);
            if (storageFile == null)
                return null;
            return await FileIO.ReadTextAsync(storageFile);
        }
    }
}