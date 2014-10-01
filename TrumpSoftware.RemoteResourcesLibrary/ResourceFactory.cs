namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal static class ResourceFactory
    {
        internal static Resource CreateResource(ResourceManager resourceManager, ResourceInfo localResourceInfo, int remoteVersion = 0)
        {
            switch (localResourceInfo.Type)
            {
                case @"text":
                    return GetTextResource(resourceManager, localResourceInfo, remoteVersion);
                default:
                    return GetTextResource(resourceManager, localResourceInfo, remoteVersion);
            }
        }

        private static Resource GetTextResource(ResourceManager resourceManager, ResourceInfo localResourceInfo, int remoteVersion = 0)
        {
            return new TextResource(resourceManager, localResourceInfo, remoteVersion);
        }
    }
}