using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public sealed class ResourceManager
    {
        private readonly static HttpClient HttpClient = new HttpClient();
        private readonly IDictionary<string, Resource> _resources = new Dictionary<string, Resource>();
        private bool _isLoaded;
        private readonly string _indexFileName;
        private IndexResource _indexResource;
        
        internal StorageFolder StorageFolder { get; private set; }
        internal string ResourcesFolderPath { get; private set; }
        internal Uri RemoteResourcesFolderUri { get; private set; }
        public bool HasInternetConnection { get; private set; }

        public ResourceManager(StorageFolder storageFolder, string resourcesFolderPath, Uri remoteResourcesFolderUri, string indexFileName = @"index.txt")
        {
            if (storageFolder == null) 
                throw new ArgumentNullException("storageFolder");
            if (resourcesFolderPath == null) 
               throw new ArgumentNullException("resourcesFolderPath");
            if (remoteResourcesFolderUri == null) 
                throw new ArgumentNullException("remoteResourcesFolderUri");
            if (indexFileName == null) 
                throw new ArgumentNullException("indexFileName");
            StorageFolder = storageFolder;
            ResourcesFolderPath = resourcesFolderPath;
            RemoteResourcesFolderUri = remoteResourcesFolderUri;
            _indexFileName = indexFileName;
        }

        public async Task LoadIndex()
        {
            _indexResource = new IndexResource(this, _indexFileName);
            await _indexResource.Load();

            _resources.Clear();

            var localIndexPath = Path.Combine(ResourcesFolderPath, _indexFileName);
            var localResourceInfos = IndexResource.Read(localIndexPath);

            IList<ResourceInfo> remoteResourceInfos = null;
            try
            {
                var remoteIndexUri = new Uri(RemoteResourcesFolderUri, _indexFileName);
                var remoteIndexString = await HttpClient.GetStringAsync(remoteIndexUri);
                remoteResourceInfos = IndexResource.Read(remoteIndexString);
                HasInternetConnection = true;
            }
            catch
            {
                HasInternetConnection = false;
            }

            if (HasInternetConnection)
            {
                foreach (var localResourceInfo in localResourceInfos)
                {
                    var localPath = localResourceInfo.LocalPath;
                    var remoteResourceInfo = remoteResourceInfos.FirstOrDefault(x => x.LocalPath == localPath);
                    Resource resource = ResourceFactory.CreateResource(this, localResourceInfo, remoteResourceInfo);
                    _resources.Add(localPath, resource);
                }
            }

            _isLoaded = true;
        }

        public async Task<T> GetResource<T>(string localPath)
            where T : class
        {
            if (!_isLoaded)
                return null;
            if (!_resources.ContainsKey(localPath))
                return null;
            var resource = _resources[localPath];
            var data = await resource.Get();
            return data as T;
        }

        public async Task LoadResource(string localPath)
        {
            if (!_isLoaded)
                return;
            if (!_resources.ContainsKey(localPath))
                return;
            var resource = _resources[localPath];
            await resource.Load();
        }
    }
}
