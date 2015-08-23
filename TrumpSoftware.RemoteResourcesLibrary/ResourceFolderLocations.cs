using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public class ResourceFolderLocations : IResourceFolderLocations
    {
        public Uri Compiled { get; private set; }
        public Uri Local { get; private set; }
        public Uri Remote { get; private set; }

        public ResourceFolderLocations(Uri compiled, Uri local, Uri remote)
        {
            Compiled = compiled;
            Local = local;
            Remote = remote;
        }
    }
}