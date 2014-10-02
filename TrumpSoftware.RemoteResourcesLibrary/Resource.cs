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
        private readonly ResourceFolderLocations _resourceFolderLocations;
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

        internal Resource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, int remoteVersion = 0)
        {
            if (resourceFolderLocations == null)
                throw new ArgumentNullException("resourceFolderLocations");
            if (localResourceInfo == null)
                throw new ArgumentNullException("localResourceInfo");
            _resourceFolderLocations = resourceFolderLocations;
            _relativePath = localResourceInfo.RelativePath;
            _localVersion = localResourceInfo.Version;
            _remoteVersion = remoteVersion;
        }

        internal async Task<object> Get(bool hasInternetConnection)
        {
            if (_data == null)
                await Load(hasInternetConnection);
            return _data;
        }

        internal async Task Load(bool hasInternetConnection)
        {
            if (_data != null)
                return;

            var relativePath = Uri.IsWellFormedUriString(_relativePath, UriKind.Relative)
                ? _relativePath
                : Uri.EscapeDataString(_relativePath);

            var remoteUri = new Uri(_resourceFolderLocations.Remote, relativePath);
            var localUri = new Uri(_resourceFolderLocations.Local, relativePath);
            var compiledUri = new Uri(_resourceFolderLocations.Compiled, relativePath);

            if (hasInternetConnection && !HasTheNewestVersion)
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
            //TODO: Заменить прямым копированием файла
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
            // TODO: Заменить на создание файла
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