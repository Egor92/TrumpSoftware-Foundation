namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal static class ResourceFactory
    {
        internal static Resource CreateResource(ResourceManager resourceManager, ResourceInfo localResourceInfo, ResourceInfo remoteResourceInfo = null)
        {
            switch (localResourceInfo.Type)
            {
                case @"text":
                    return new TextResource(resourceManager, localResourceInfo, remoteResourceInfo);
                default:
                    return new TextResource(resourceManager, localResourceInfo, remoteResourceInfo);
            }
        }
    }
}