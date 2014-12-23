using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class UriResourceConverter : IResourceConverter<Uri>
    {
        public Uri Convert(IResource resource)
        {
            return resource.GetUri();
        }
    }
}