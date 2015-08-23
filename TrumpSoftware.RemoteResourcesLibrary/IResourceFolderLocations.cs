using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public interface IResourceFolderLocations
    {
        Uri Compiled { get; }
        Uri Local { get; }
        Uri Remote { get; }
    }
}