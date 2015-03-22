using System;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    internal class ResourceInfo
    {
        public string RelativePath { get; set; }
        public string Group { get; set; }
        public int Version { get; set; }

        public ResourceInfo(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("relativePath");
            RelativePath = relativePath;
            Group = null;
            Version = 0;
        }
    }
}