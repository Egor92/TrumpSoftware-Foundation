namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal static class ResourceFactory
    {
        internal static Resource CreateResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo)
        {
            switch (localResourceInfo.Type)
            {
                case @"text":
                    return GetTextResource(resourceFolderLocations, localResourceInfo, remoteResourceInfo);
                default:
                    return GetTextResource(resourceFolderLocations, localResourceInfo, remoteResourceInfo);
            }
        }

        internal static Resource CreateResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo remoteResourceInfo)
        {
            var localResourceInfo = new ResourceInfo(remoteResourceInfo)
            {
                Version = 0
            };
            return CreateResource(resourceFolderLocations, localResourceInfo, remoteResourceInfo);
        }

        private static Resource GetTextResource(ResourceFolderLocations resourceFolderLocations, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo)
        {
            return new TextResource(resourceFolderLocations, localResourceInfo, remoteResourceInfo);
        }
    }
}