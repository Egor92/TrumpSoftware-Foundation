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
        private readonly ResourceFolderLocations _resourceFolderLocations;
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

        internal Index(ResourceFolderLocations resourceFolderLocations, string indexFileName)
        {
            _resourceFolderLocations = resourceFolderLocations;
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
            try
            {
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(LocalIndexUri);
                if (storageFile == null)
                    return null;
                var indexData = await FileIO.ReadTextAsync(storageFile);
                var resourceInfos = Read(indexData);
                return resourceInfos;
            }
            catch
            {
                return null;
            }
        }

        private async Task<IList<ResourceInfo>> GetCompiledIndex()
        {
            try
            {
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(CompiledIndexUri);
                if (storageFile == null)
                    return null;

                var indexData = await FileIO.ReadTextAsync(storageFile);
                var resourceInfos = Read(indexData);
                return resourceInfos;
            }
            catch
            {
                return null;
            }
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
