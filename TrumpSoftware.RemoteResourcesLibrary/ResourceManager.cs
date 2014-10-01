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

        internal Uri CompiledResourceFolderUri { get; private set; }
        internal Uri LocalResourcesFolderUri { get; private set; }
        internal Uri RemoteResourcesFolderUri { get; private set; }
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
            CompiledResourceFolderUri = compiledResourceFolderUri;
            LocalResourcesFolderUri = localResourcesFolderUri;
            RemoteResourcesFolderUri = remoteResourcesFolderUri;
            _index = new Index(this, indexFileName);
        }

        public async Task LoadIndex()
        {
            _resources.Clear();
            await _index.Load();

            if (!_index.IsLocalResourceInfoLoaded)
            {
                IsLoaded = false;
                return;
            }

            var localResourceInfos = _index.LocalResourceInfos;

            foreach (var localResourceInfo in localResourceInfos)
            {
                var localPath = localResourceInfo.RelativePath;
                int remoteResourceVersion = 0;
                if (_index.IsRemoteResourceInfoDownloaded)
                {
                    var remoteResourceInfo = _index.RemoteResourceInfos.FirstOrDefault(x => x.RelativePath == localPath);
                    if (remoteResourceInfo != null)
                        remoteResourceVersion = remoteResourceInfo.Version;
                }
                Resource resource = ResourceFactory.CreateResource(this, localResourceInfo, remoteResourceVersion);
                _resources.Add(localPath, resource);
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
            var data = await resource.Get();
            return data as T;
        }

        public async Task LoadResource(string path)
        {
            if (!IsLoaded)
                return;
            if (!_resources.ContainsKey(path))
                return;
            var resource = _resources[path];
            await resource.Load();
        }
    }
}
