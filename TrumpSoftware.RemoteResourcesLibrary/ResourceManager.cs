using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public sealed class ResourceManager
    {
        private readonly IDictionary<string, Resource> _resources = new Dictionary<string, Resource>();
        private readonly Index _index;

        internal ResourceFolderLocations ResourceFolderLocations { get; private set; }
        public bool HasInternetConnection { get; private set; }
        public bool IsLoaded { get; private set; }

        public ResourceManager(Uri compiledResourceFolderUri, Uri localResourcesFolderUri, Uri remoteResourcesFolderUri, string indexFileName = @"index.txt")
        {
            if (compiledResourceFolderUri == null) 
                throw new ArgumentNullException("compiledResourceFolderUri");
            if (localResourcesFolderUri == null) 
               throw new ArgumentNullException("localResourcesFolderUri");
            if (remoteResourcesFolderUri == null) 
                throw new ArgumentNullException("remoteResourcesFolderUri");
            if (indexFileName == null) 
                throw new ArgumentNullException("indexFileName");
            ResourceFolderLocations = new ResourceFolderLocations(compiledResourceFolderUri, localResourcesFolderUri, remoteResourcesFolderUri);
            _index = new Index(ResourceFolderLocations, indexFileName);
        }

        public async Task LoadIndexAsync()
        {
            _resources.Clear();
            if (_index.IsLocalResourceInfoLoaded)
                await SaveIndexAsync();
            await _index.LoadAsync();

            foreach (var remoteResourceInfo in _index.RemoteResourceInfos)
            {
                var relativePath = remoteResourceInfo.RelativePath;
                var localResourceInfo = _index.LocalResourceInfos.SingleOrDefault(x => x.RelativePath == relativePath);
                Resource resource = localResourceInfo != null
                    ? ResourceFactory.CreateResource(ResourceFolderLocations, localResourceInfo, remoteResourceInfo)
                    : ResourceFactory.CreateResource(ResourceFolderLocations, remoteResourceInfo);
                _resources.Add(relativePath, resource);
            }
            await SaveIndexAsync();

            HasInternetConnection = _index.IsRemoteResourceInfoDownloaded;
            IsLoaded = true;
        }

        public async Task SaveIndexAsync()
        {
            if (_index.IsLocalResourceInfoLoaded)
            {
                var resourceInfos = _resources.Select(x => x.Value.GetResourceInfo());
                await _index.SaveAsync(resourceInfos);
            }
        }

        public async Task<T> GetResourceAsync<T>(string path)
            where T : class
        {
            if (!IsLoaded)
                return null;
            if (!_resources.ContainsKey(path))
                return null;
            var resource = _resources[path];
            var data = await resource.GetAsync(HasInternetConnection);
            return data as T;
        }

        public async Task<IEnumerable<T>> GetResourceGroupAsync<T>(string groupName)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return null;
            if (!IsLoaded)
                return null;
            var resorces = _resources.Where(x => x.Value.Group == groupName).Select(x => x.Value);
            var datas = new List<T>();
            foreach (var resource in resorces)
            {
                var data = await resource.GetAsync(HasInternetConnection);
                var t = data as T;
                datas.Add(t);
            }
            return datas;
        }

        public async Task LoadResourceAsync(string path)
        {
            if (!IsLoaded)
                return;
            if (!_resources.ContainsKey(path))
                return;
            var resource = _resources[path];
            await resource.LoadAsync(HasInternetConnection);
        }

        public async Task LoadResourceGroupAsync(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return;
            if (!IsLoaded)
                return;
            var resorces = _resources.Where(x => x.Value.Group == groupName).Select(x => x.Value);
            foreach (var resource in resorces)
                await resource.LoadAsync(HasInternetConnection);
        }
    }
}
