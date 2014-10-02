using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class TextResource : Resource
    {
        public TextResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, int remoteVersion = 0)
            : base(resourceFolderLocations, localResourceInfo, remoteVersion)
        {
        }

        protected override async Task<object> LoadLocalResource(Uri uri)
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            if (storageFile == null)
                return null;
            return await FileIO.ReadTextAsync(storageFile);
        }
    }
}
