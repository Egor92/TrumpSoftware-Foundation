namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal static class ResourceFactory
    {
        internal static Resource CreateResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, int remoteVersion = 0)
        {
            switch (localResourceInfo.Type)
            {
                case @"text":
                    return GetTextResource(resourceFolderLocations, localResourceInfo, remoteVersion);
                default:
                    return GetTextResource(resourceFolderLocations, localResourceInfo, remoteVersion);
            }
        }

        internal static Resource CreateResourceFromRemoteResourceInfo(ResourceFolderLocations resourceFolderLocations, ResourceInfo remoteResourceInfo)
        {
            int remoteVersion = remoteResourceInfo.Version;
            remoteResourceInfo.Version = 0;

            return CreateResource(resourceFolderLocations, remoteResourceInfo, remoteVersion);
        }

        private static Resource GetTextResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, int remoteVersion = 0)
        {
            return new TextResource(resourceFolderLocations, localResourceInfo, remoteVersion);
        }
    }
}