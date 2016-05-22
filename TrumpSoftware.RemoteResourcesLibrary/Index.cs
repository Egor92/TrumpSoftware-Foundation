using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;
using TrumpSoftware.Common.PCLStorage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class Index
    {
        private readonly IResourceFolderLocations _resourceFolderLocations;
        private readonly string _indexFileName;
        private static readonly HttpClient HttpClient = new HttpClient();

        internal bool IsLocalResourceInfoLoaded
        {
            get { return LocalResourceInfos != null; }
        }

        internal bool IsRemoteResourceInfoDownloaded
        {
            get { return RemoteResourceInfos != null; }
        }

        internal IEnumerable<ResourceInfo> LocalResourceInfos { get; private set; }

        internal IEnumerable<ResourceInfo> RemoteResourceInfos { get; private set; }

        private Uri CompiledIndexUri
        {
            get { return new Uri(_resourceFolderLocations.Compiled, _indexFileName); }
        }

        private Uri LocalIndexUri
        {
            get { return new Uri(_resourceFolderLocations.Local, _indexFileName); }
        }

        private Uri RemoteIndexUri
        {
            get { return new Uri(_resourceFolderLocations.Remote, _indexFileName); }
        }

        internal Index(IResourceFolderLocations resourceFolderLocations, string indexFileName)
        {
            _resourceFolderLocations = resourceFolderLocations;
            _indexFileName = indexFileName;
        }

        internal async Task LoadAsync()
        {
            LocalResourceInfos = await LoadLocalIndexAsync();
            if (LocalResourceInfos == null)
                LocalResourceInfos = await GetCompiledIndexAsync();
            RemoteResourceInfos = await DownloadRemoteIndexAsync();
        }

        internal async Task SaveAsync(IEnumerable<ResourceInfo> resourceInfos)
        {
            var indexData = Write(resourceInfos);
            var file = await PathHelper.GetOrCreateFileAsync(LocalIndexUri.LocalPath);
            await file.WriteAllTextAsync(indexData);
        }

        private async Task<IList<ResourceInfo>> DownloadRemoteIndexAsync()
        {
            string indexData;
            try
            {
                indexData = await HttpClient.GetStringAsync(RemoteIndexUri);
                var resourceInfos = Read(indexData);
                return resourceInfos;
            }
            catch
            {
                return null;
            }
        }

        private async Task<IList<ResourceInfo>> LoadLocalIndexAsync()
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(LocalIndexUri.LocalPath, CancellationToken.None);
            if (file == null)
                return null;
            var indexData = await file.ReadAllTextAsync();
            var resourceInfos = Read(indexData);
            return resourceInfos;
        }

        private async Task<IList<ResourceInfo>> GetCompiledIndexAsync()
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(CompiledIndexUri.LocalPath, CancellationToken.None);
            if (file == null)
                return null;
            var indexData = await file.ReadAllTextAsync();
            var resourceInfos = Read(indexData);
            return resourceInfos;
        }

        internal static string Write(IEnumerable<ResourceInfo> resources)
        {
            return JsonConvert.SerializeObject(resources);
        }

        internal static IList<ResourceInfo> Read(string data)
        {
            return JsonConvert.DeserializeObject<List<ResourceInfo>>(data);
        }
    }
}
