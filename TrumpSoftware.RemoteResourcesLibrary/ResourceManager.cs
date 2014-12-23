using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrumpSoftware.Xaml.Media;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public class ResourceManager
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

            if (_index.IsRemoteResourceInfoDownloaded)
            {
                foreach (var remoteResourceInfo in _index.RemoteResourceInfos)
                {
                    var relativePath = remoteResourceInfo.RelativePath;
                    var localResourceInfo = _index.LocalResourceInfos.SingleOrDefault(x => x.RelativePath == relativePath);
                    Resource resource = ResourceFactory.CreateResource(ResourceFolderLocations, localResourceInfo, remoteResourceInfo);
                    _resources.Add(relativePath, resource);
                }
            }
            else
            {
                foreach (var localResourceInfo in _index.LocalResourceInfos)
                {
                    var relativePath = localResourceInfo.RelativePath;
                    Resource resource = ResourceFactory.CreateResource(ResourceFolderLocations, localResourceInfo, null);
                    _resources.Add(relativePath, resource);
                }
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

        public async Task<string> GetStringResourceAsync(string path)
        {
            return await GetResourceAsync<string>(path);
        }

        public async Task<int> GetIntegerResourceAsync(string path)
        {
            return await GetResourceAsync<int>(path);
        }

        public async Task<double> GetDoubleResourceAsync(string path)
        {
            return await GetResourceAsync<double>(path);
        }

        public async Task<Uri> GetUriResourceAsync(string path)
        {
            return await GetResourceAsync<Uri>(path);
        }

        public async Task<MediaObject> GetMediaObjectResourceAsync(string path)
        {
            return await GetResourceAsync<MediaObject>(path);
        }

        private async Task<T> GetResourceAsync<T>(string path)
        {
            if (!IsLoaded)
                throw new Exception("ResourceManager didn't loaded");
            if (!_resources.ContainsKey(path))
                throw new Exception(string.Format("Resource with path \"{0}\" is absent", path));
            var resource = _resources[path];
            return await resource.GetAsync<T>(HasInternetConnection);
        }

        public async Task<IEnumerable<string>> GetStringResourceGroupAsync(string groupName)
        {
            return await GetResourceGroupAsync<string>(groupName);
        }

        public async Task<IEnumerable<int>> GetIntegerResourceGroupAsync(string groupName)
        {
            return await GetResourceGroupAsync<int>(groupName);
        }

        public async Task<IEnumerable<double>> GetDoubleResourceGroupAsync(string groupName)
        {
            return await GetResourceGroupAsync<double>(groupName);
        }

        public async Task<IEnumerable<Uri>> GetUriResourceGroupAsync(string groupName)
        {
            return await GetResourceGroupAsync<Uri>(groupName);
        }

        public async Task<IEnumerable<MediaObject>> GetMediaObjectResourceGroupAsync(string groupName)
        {
            return await GetResourceGroupAsync<MediaObject>(groupName);
        }

        private async Task<IEnumerable<T>> GetResourceGroupAsync<T>(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return null;
            if (!IsLoaded)
                return null;
            var resorces = _resources.Where(x => x.Value.Group == groupName).Select(x => x.Value);
            var datas = new List<T>();
            foreach (var resource in resorces)
            {
                var data = await resource.GetAsync<T>(HasInternetConnection);
                datas.Add(data);
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

        public async Task LoadAllResourcesAsync()
        {
            if (!IsLoaded)
                return;
            var resorces = _resources.Select(x => x.Value);
            foreach (var resource in resorces)
                await resource.LoadAsync(HasInternetConnection);
        }
    }
}
