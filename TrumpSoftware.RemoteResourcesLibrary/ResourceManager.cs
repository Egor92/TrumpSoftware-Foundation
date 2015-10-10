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
        private readonly IResourceFolderLocations _resourceFolderLocations;
        private bool _hasInternetConnection;
        private bool _isLoaded;

        public ResourceManager(IResourceFolderLocations resourceFolderLocations, string indexFileName = @"index.txt")
        {
            if (resourceFolderLocations == null)
                throw new ArgumentNullException("resourceFolderLocations");
            if (indexFileName == null) 
                throw new ArgumentNullException("indexFileName");
            _resourceFolderLocations = resourceFolderLocations;
            _index = new Index(_resourceFolderLocations, indexFileName);
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
                    Resource resource = ResourceFactory.CreateResource(_resourceFolderLocations, localResourceInfo, remoteResourceInfo);
                    _resources.Add(relativePath, resource);
                }
            }
            else
            {
                foreach (var localResourceInfo in _index.LocalResourceInfos)
                {
                    var relativePath = localResourceInfo.RelativePath;
                    Resource resource = ResourceFactory.CreateResource(_resourceFolderLocations, localResourceInfo, null);
                    _resources.Add(relativePath, resource);
                }
            }
            await SaveIndexAsync();

            _hasInternetConnection = _index.IsRemoteResourceInfoDownloaded;
            _isLoaded = true;
        }

        public async Task SaveIndexAsync()
        {
            if (_index.IsLocalResourceInfoLoaded)
            {
                var resourceInfos = _resources.Select(x => x.Value.GetResourceInfo());
                await _index.SaveAsync(resourceInfos);
            }
        }

        public bool ContainsResource(string key)
        {
            return _resources.ContainsKey(key);
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
            if (!_isLoaded)
                throw new Exception("ResourceManager didn't loaded");
            if (!_resources.ContainsKey(path))
                throw new Exception(string.Format("Resource with path \"{0}\" is absent", path));
            var resource = _resources[path];
            return await resource.GetAsync<T>(_hasInternetConnection);
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
            if (!_isLoaded)
                return null;
            var resorces = _resources.Where(x => x.Value.Group == groupName).Select(x => x.Value);
            var datas = new List<T>();
            foreach (var resource in resorces)
            {
                var data = await resource.GetAsync<T>(_hasInternetConnection);
                datas.Add(data);
            }
            return datas;
        }

        public async Task LoadResourceAsync(string path)
        {
            if (!_isLoaded)
                return;
            if (!_resources.ContainsKey(path))
                return;
            var resource = _resources[path];
            await resource.LoadAsync(_hasInternetConnection);
        }

        public async Task LoadResourceGroupAsync(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return;
            if (!_isLoaded)
                return;
            var resorces = _resources.Where(x => x.Value.Group == groupName).Select(x => x.Value);
            foreach (var resource in resorces)
                await resource.LoadAsync(_hasInternetConnection);
        }

        public async Task LoadAllResourcesAsync()
        {
            if (!_isLoaded)
                return;
            var resorces = _resources.Select(x => x.Value);
            foreach (var resource in resorces)
                await resource.LoadAsync(_hasInternetConnection);
        }
    }
}
