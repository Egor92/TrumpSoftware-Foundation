using System;
using Newtonsoft.Json;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal sealed class ResourceInfo
    {
        public string RelativePath { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }

        internal ResourceInfo()
        {
        }

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