using System;
using System.Net.Http;
using System.Threading.Tasks;
using PCLStorage;
using TrumpSoftware.Common;
using TrumpSoftware.Common.PCLStorage;
using FileExtensions = TrumpSoftware.Common.PCLStorage.FileExtensions;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal abstract class Resource
    {
        private object _data;
        private readonly ResourceFolderLocations _resourceFolderLocations;
        private readonly ResourceInfo _localResourceInfo;
        private readonly ResourceInfo _remoteResourceInfo;

        private static readonly HttpClient HttpClient = new HttpClient();

        private bool HasTheNewestVersion 
        {
            get { return _localResourceInfo.Version >= _remoteResourceInfo.Version; }
        }

        internal string Group
        {
            get { return _remoteResourceInfo.Group; }
        }

        internal bool IsLoaded
        {
            get { return _data != null; }
        }

        internal Resource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo)
        {
            if (resourceFolderLocations == null)
                throw new ArgumentNullException("resourceFolderLocations");
            if (localResourceInfo == null)
                throw new ArgumentNullException("localResourceInfo");
            if (remoteResourceInfo == null)
                throw new ArgumentNullException("remoteResourceInfo");
            _resourceFolderLocations = resourceFolderLocations;
            _localResourceInfo = localResourceInfo;
            _remoteResourceInfo = remoteResourceInfo;
        }

        internal async Task<object> GetAsync(bool hasInternetConnection)
        {
            if (_data == null)
                await LoadAsync(hasInternetConnection);
            return _data;
        }

        internal async Task LoadAsync(bool hasInternetConnection)
        {
            if (_data != null)
                return;

            var relativePath = _remoteResourceInfo.RelativePath;

            var remoteUri = new Uri(_resourceFolderLocations.Remote, relativePath);
            var localUri = new Uri(_resourceFolderLocations.Local, relativePath);
            var compiledUri = new Uri(_resourceFolderLocations.Compiled, relativePath);

            if (hasInternetConnection && !HasTheNewestVersion)
            {
                var downloadedSuccessfully = await DownloadResourceAsync(remoteUri, localUri);
                if (downloadedSuccessfully)
                    _data = await LoadLocalResourceAsync(localUri);
            }
            else if (_data == null)
                _data = await LoadLocalResourceAsync(localUri);

            if (_data == null)
            {
                await CopyFromCompiledResourceAsync(compiledUri, localUri);
                _data = await LoadLocalResourceAsync(localUri);
            }
        }

        internal ResourceInfo GetResourceInfo()
        {
            return _data == null
                ? _localResourceInfo
                : _remoteResourceInfo;
        }

        private static async Task<bool> DownloadResourceAsync(Uri fromUri, Uri toUri)
        {
            string str;
            try
            {
                str = await HttpClient.GetStringAsync(fromUri);
            }
            catch
            {
                return false;
            }
            var file = await PathHelper.GetOrCreateFileAsync(toUri.LocalPath);
            await file.WriteAllTextAsync(str);
            return true;
        }

        protected abstract Task<object> LoadLocalResourceAsync(Uri uri);

        private static async Task CopyFromCompiledResourceAsync(Uri fromUri, Uri toUri)
        {
            var copySourceFile = await FileSystem.Current.GetFileFromPathAsync(fromUri.LocalPath);
            await copySourceFile.CopyFileAsync(toUri.LocalPath);
        }
    }
}