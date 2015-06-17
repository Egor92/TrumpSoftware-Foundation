using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PCLStorage;
using TrumpSoftware.Common;
using TrumpSoftware.Common.Helpers;
using TrumpSoftware.Common.PCLStorage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class Resource : IResource
    {
        private readonly ResourceFolderLocations _resourceFolderLocations;
        private readonly ResourceInfo _localResourceInfo;
        private readonly ResourceInfo _remoteResourceInfo;
        private bool _isRemoteVersionDownloaded;

        private static readonly HttpClient HttpClient = new HttpClient();

        private bool HasTheNewestVersion 
        {
            get { return _localResourceInfo.Version >= _remoteResourceInfo.Version; }
        }

        private Uri RemoteUri
        {
            get { return new Uri(_resourceFolderLocations.Remote, _remoteResourceInfo.RelativePath); }
        }

        private string LocalPath
        {
            get { return new Uri(_resourceFolderLocations.Local, _remoteResourceInfo.RelativePath).LocalPath; }
        }

        private string CompiledPath
        {
            get { return new Uri(_resourceFolderLocations.Compiled, _remoteResourceInfo.RelativePath).LocalPath; }
        }

        internal string Group
        {
            get { return _remoteResourceInfo.Group; }
        }

        internal bool IsLoaded { get; private set; }

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

        internal async Task<T> GetAsync<T>(bool hasInternetConnection)
        {
            if (!IsLoaded)
                await LoadAsync(hasInternetConnection);
            var resourceConverter = ResourceConverterFactory.GetConverter<T>();
            return resourceConverter.Convert(this);
        }

        internal async Task LoadAsync(bool hasInternetConnection)
        {
            if (IsLoaded)
                return;

            _isRemoteVersionDownloaded = false;
            if (hasInternetConnection && !HasTheNewestVersion)
            {
                _isRemoteVersionDownloaded = await DownloadResourceAsync();
                if (_isRemoteVersionDownloaded)
                    IsLoaded = await CheckResourceFileExistAsync();
            }
            else if (!IsLoaded)
                IsLoaded = await CheckResourceFileExistAsync();

            if (!IsLoaded)
            {
                await CopyFromCompiledResourceAsync();
                IsLoaded = await CheckResourceFileExistAsync();
            }
        }

        internal ResourceInfo GetResourceInfo()
        {
            return _isRemoteVersionDownloaded
                ? _remoteResourceInfo
                : _localResourceInfo;
        }

        private async Task<bool> CheckResourceFileExistAsync()
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(LocalPath);
            return file != null;
        }

        private async Task<bool> DownloadResourceAsync()
        {
            string str;
            try
            {
                str = await HttpClient.GetStringAsync(RemoteUri);
            }
            catch
            {
                return false;
            }
            var file = await PathHelper.GetOrCreateFileAsync(LocalPath);
            await file.WriteAllTextAsync(str);
            return true;
        }

        private async Task CopyFromCompiledResourceAsync()
        {
            var copySourceFile = await FileSystem.Current.GetFileFromPathAsync(CompiledPath);
            await copySourceFile.CopyFileAsync(LocalPath);
        }

        async Task<Stream> IResource.GetStreamAsync()
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(LocalPath).ConfigureAwait(false);
            if (file == null)
                return null;
            return await file.OpenAsync(FileAccess.Read).ConfigureAwait(false);
        }

        Uri IResource.GetUri()
        {
            return new Uri(_resourceFolderLocations.Local, _remoteResourceInfo.RelativePath);
        }
    }
}