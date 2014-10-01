using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal abstract class Resource
    {
        private object _data;
        private readonly ResourceManager _resourceManager;
        private readonly string _relativePath;
        private int _localVersion;
        private readonly int _remoteVersion;

        private static readonly HttpClient HttpClient = new HttpClient();

        private bool HasTheNewestVersion 
        {
            get { return _localVersion >= _remoteVersion; }
        }

        internal bool IsLoaded
        {
            get { return _data != null; }
        }

        internal Resource(ResourceManager resourceManager, ResourceInfo localResourceInfo, int remoteVersion = 0)
        {
            if (resourceManager == null) 
               throw new ArgumentNullException("resourceManager");
            if (localResourceInfo == null)
                throw new ArgumentNullException("localResourceInfo");
            _resourceManager = resourceManager;
            _relativePath = localResourceInfo.RelativePath;
            _localVersion = localResourceInfo.Version;
            _remoteVersion = remoteVersion;
        }

        internal async Task<object> Get()
        {
            if (_data == null)
                await Load();
            return _data;
        }

        internal async Task Load()
        {
            if (_data != null)
                return;
            if (!Uri.IsWellFormedUriString(_relativePath, UriKind.Relative))
                return;

            var remoteUri = new Uri(_resourceManager.RemoteResourcesFolderUri, _relativePath);
            var localUri = new Uri(_resourceManager.LocalResourcesFolderUri, _relativePath);
            var compiledUri = new Uri(_resourceManager.CompiledResourceFolderUri, _relativePath);

            if (_resourceManager.HasInternetConnection && !HasTheNewestVersion)
            {
                var downloadedSuccessfully = await DownloadResource(remoteUri, localUri);
                if (downloadedSuccessfully)
                    _data = await LoadLocalResource(localUri);
                if (_data != null)
                    UpdateLocalVersion();
            }
            else if (_data == null)
                _data = await LoadLocalResource(localUri);

            if (_data == null)
            {
                await CopyFromCompiledResource(compiledUri, localUri);
                _data = await LoadLocalResource(localUri);
            }
        }

        private static async Task<bool> DownloadResource(Uri fromUri, Uri toUri)
        {
            byte[] buffer;
            try
            {
                buffer = await HttpClient.GetByteArrayAsync(fromUri);
            }
            catch
            {
                return false;
            }
            var savedSuccessfully = await SaveToLocalFile(toUri, buffer);
            return savedSuccessfully;
        }

        protected abstract Task<object> LoadLocalResource(Uri uri);

        private static async Task<bool> CopyFromCompiledResource(Uri fromUri, Uri toUri)
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(fromUri);
            if (storageFile == null)
                return false;
            byte[] buffer;
            using (var stream = await storageFile.OpenReadAsync())
            {
                buffer = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(buffer);
                }
            }
            var savedSuccessfully = await SaveToLocalFile(toUri, buffer);
            return savedSuccessfully;
        }

        private static async Task<bool> SaveToLocalFile(Uri uri, byte[] buffer)
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            if (storageFile == null)
                return false;
            await FileIO.WriteBytesAsync(storageFile, buffer);
            return true;
        }

        private void UpdateLocalVersion()
        {
            _localVersion = _remoteVersion;
        }
    }
}