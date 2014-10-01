using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class Index
    {
        private readonly ResourceManager _resourceManager;
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

        internal IList<ResourceInfo> LocalResourceInfos { get; private set; }

        internal IList<ResourceInfo> RemoteResourceInfos { get; private set; }

        private Uri CompiledIndexUri
        {
            get { return new Uri(_resourceManager.CompiledResourceFolderUri, _indexFileName); }
        }

        private Uri LocalIndexUri
        {
            get { return new Uri(_resourceManager.RemoteResourcesFolderUri, _indexFileName); }
        }

        private Uri RemoteIndexUri
        {
            get { return new Uri(_resourceManager.RemoteResourcesFolderUri, _indexFileName); }
        }

        internal Index(ResourceManager resourceManager, string indexFileName)
        {
            _resourceManager = resourceManager;
            _indexFileName = indexFileName;
        }

        internal async Task Load()
        {
            LocalResourceInfos = await LoadLocalIndex();
            if (LocalResourceInfos == null)
                LocalResourceInfos = await GetCompiledIndex();
            RemoteResourceInfos = await DownloadRemoteIndex();
        }

        private async Task<IList<ResourceInfo>> DownloadRemoteIndex()
        {
            string indexData;
            try
            {
                indexData = await HttpClient.GetStringAsync(RemoteIndexUri);
            }
            catch
            {
                return null;
            }
            var resourceInfos = Read(indexData);
            return resourceInfos;
        }

        private async Task<IList<ResourceInfo>> LoadLocalIndex()
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(LocalIndexUri);
            if (storageFile == null)
                return null;
            var indexData = await FileIO.ReadTextAsync(storageFile);
            var resourceInfos = Read(indexData);
            return resourceInfos;
        }

        private async Task<IList<ResourceInfo>> GetCompiledIndex()
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(CompiledIndexUri);
            if (storageFile == null)
                return null;
            var indexData = await FileIO.ReadTextAsync(storageFile);
            var resourceInfos = Read(indexData);
            return resourceInfos;
        }

        internal static void Write(IEnumerable<ResourceInfo> resources)
        {
            JsonConvert.SerializeObject(resources);
        }

        internal static IList<ResourceInfo> Read(string data)
        {
            return JsonConvert.DeserializeObject<List<ResourceInfo>>(data);
        }
    }
}
