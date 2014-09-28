using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal sealed class ResourceInfo
    {
        internal string LocalPath { get; set; }
        internal Uri RemotePath { get; set; }
        internal string Type { get; set; }
        internal int Version { get; set; }

        internal ResourceInfo(string localPath, Uri remotePath, string type, int version)
        {
            if (localPath == null) 
               throw new ArgumentNullException("localPath");
            if (remotePath == null) 
               throw new ArgumentNullException("remotePath");
            if (type == null)
                throw new ArgumentNullException("type");
            LocalPath = localPath;
            RemotePath = remotePath;
            Type = type;
            Version = version;
        }
    }
}