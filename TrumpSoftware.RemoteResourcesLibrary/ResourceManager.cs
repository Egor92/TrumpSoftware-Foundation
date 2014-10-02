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
        public bool IsLoaded;

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

        public async Task LoadIndex()
        {
            _resources.Clear();
            await _index.Load();

            var localResourceInfos = _index.LocalResourceInfos;
            var remoteResourceInfos = new List<ResourceInfo>(_index.RemoteResourceInfos);
            if (localResourceInfos != null)
            {
                foreach (var localResourceInfo in localResourceInfos)
                {
                    var localPath = localResourceInfo.RelativePath;
                    int remoteResourceVersion = 0;
                    if (_index.IsRemoteResourceInfoDownloaded)
                    {
                        var remoteResourceInfo =
                            _index.RemoteResourceInfos.FirstOrDefault(x => x.RelativePath == localPath);
                        if (remoteResourceInfo != null)
                        {
                            remoteResourceInfos.Remove(remoteResourceInfo);
                            remoteResourceVersion = remoteResourceInfo.Version;
                        }
                    }
                    Resource resource = ResourceFactory.CreateResource(ResourceFolderLocations, localResourceInfo,
                        remoteResourceVersion);
                    _resources.Add(localPath, resource);
                }
            }

            foreach (var remoteResourceInfo in remoteResourceInfos)
            {
                Resource resource = ResourceFactory.CreateResourceFromRemoteResourceInfo(ResourceFolderLocations, remoteResourceInfo);
                _resources.Add(remoteResourceInfo.RelativePath, resource);
            }

            HasInternetConnection = _index.IsRemoteResourceInfoDownloaded;
            IsLoaded = true;
        }

        public async Task<T> GetResource<T>(string path)
            where T : class
        {
            if (!IsLoaded)
                return null;
            if (!_resources.ContainsKey(path))
                return null;
            var resource = _resources[path];
            var data = await resource.Get(HasInternetConnection);
            return data as T;
        }

        public async Task LoadResource(string path)
        {
            if (!IsLoaded)
                return;
            if (!_resources.ContainsKey(path))
                return;
            var resource = _resources[path];
            await resource.Load(HasInternetConnection);
        }
    }
}
