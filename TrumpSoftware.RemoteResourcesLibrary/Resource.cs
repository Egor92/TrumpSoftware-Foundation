using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal abstract class Resource
    {
        private object _data;
        private readonly ResourceManager _resourceManager;
        private readonly string _localPath;
        private readonly Uri _remotePath;
        private int _localVersion;
        private readonly int _remoteVersion;

        protected readonly static HttpClient HttpClient = new HttpClient();

        private bool HasTheNewestVersion 
        {
            get { return _localVersion >= _remoteVersion; }
        }

        protected Resource(ResourceManager resourceManager, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo = null)
        {
            if (resourceManager == null) 
               throw new ArgumentNullException("resourceManager");
            if (localResourceInfo == null)
                throw new ArgumentNullException("localResourceInfo");
            _resourceManager = resourceManager;
            _localPath = localResourceInfo.LocalPath;
            _localVersion = localResourceInfo.Version;
            if (remoteResourceInfo != null)
            {
                if (localResourceInfo.LocalPath != remoteResourceInfo.LocalPath)
                    throw new ArgumentException("LocalPath are not equls in local and remote ResourceInfos");
                _remotePath = remoteResourceInfo.RemotePath;
                _remoteVersion = remoteResourceInfo.Version;
            }
            else
            {
                _remotePath = localResourceInfo.RemotePath;
                _remoteVersion = 0;
            }
        }

        internal async Task<object> Get()
        {
            if (_data == null)
                await Load();
            return _data;
        }

        internal async virtual Task Load()
        {
            if (_data != null)
                return;
            if (_resourceManager.HasInternetConnection && !HasTheNewestVersion)
            {
                var remoteUri = new Uri(_resourceManager.RemoteResourcesFolderUri, _remotePath);
                try
                {
                    _data = await DownloadFromServer(remoteUri);
                }
                catch
                {
                    _data = null;
                }
                if (_data != null)
                    UpdateVersion();
            }
            if (_data == null)
            {
                _data = await LoadFromLocalStorage(_resourceManager.StorageFolder, _localPath);
            }
            if (_data == null)
            {
                var path = Path.Combine(_resourceManager.ResourcesFolderPath, _localPath);
                _data = await LoadFromCompiledResource(path);
            }
        }

        protected abstract Task<object> DownloadFromServer(Uri uri);

        protected abstract Task<object> LoadFromLocalStorage(StorageFolder storageFolder, string path);

        protected abstract Task<object> LoadFromCompiledResource(string path);

        private void UpdateVersion()
        {
            _localVersion = _remoteVersion;
        }
    }
}