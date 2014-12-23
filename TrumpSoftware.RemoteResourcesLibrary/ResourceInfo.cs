namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal sealed class ResourceInfo
    {
        public string RelativePath { get; set; }
        public string Group { get; set; }
        public int Version { get; set; }

        internal ResourceInfo()
        {
        }

        internal ResourceInfo(ResourceInfo other)
        {
            RelativePath = other.RelativePath;
            Group = other.Group;
            Version = other.Version;
        }
    }
}