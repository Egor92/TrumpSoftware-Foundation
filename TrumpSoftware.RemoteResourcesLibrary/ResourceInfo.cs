using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal sealed class ResourceInfo
    {
        internal string RelativePath { get; set; }
        internal string Type { get; set; }
        internal int Version { get; set; }

        internal ResourceInfo(string relativePath, string type, int version)
        {
            if (relativePath == null) 
               throw new ArgumentNullException("relativePath");
            if (type == null)
                throw new ArgumentNullException("type");
            RelativePath = relativePath;
            Type = type;
            Version = version;
        }
    }
}