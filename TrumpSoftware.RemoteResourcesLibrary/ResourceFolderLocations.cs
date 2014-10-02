using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class ResourceFolderLocations
    {
        internal Uri Compiled { get; private set; }
        internal Uri Local { get; private set; }
        internal Uri Remote { get; private set; }

        internal ResourceFolderLocations(Uri compiled, Uri local, Uri remote)
        {
            Compiled = compiled;
            Local = local;
            Remote = remote;
        }
    }
}