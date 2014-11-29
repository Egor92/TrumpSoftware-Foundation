using System;
using System.Threading.Tasks;
using PCLStorage;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class TextResource : Resource
    {
        internal TextResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo)
            : base(resourceFolderLocations, localResourceInfo, remoteResourceInfo)
        {
        }

        protected override async Task<object> LoadLocalResourceAsync(Uri uri)
        {
            var file = await FileSystem.Current.GetFileFromPathAsync(uri.LocalPath);
            if (file == null)
                return null;
            return await file.ReadAllTextAsync();
        }
    }
}
