namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    internal class ResourceInfo
    {
        public string RelativePath { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public int Version { get; set; }

        public bool IsAvailable()
        {
            return RelativePath != null && Version > -1;
        }

        public void AddValue(string key, string value)
        {
            switch (key)
            {
                case "RelativePath":
                    AddRelativePath(value);
                    break;
                case "Type":
                    AddType(value);
                    break;
                case "Group":
                    AddGroup(value);
                    break;
                case "Version":
                    AddVersion(value);
                    break;
            }
        }

        private void AddRelativePath(string value)
        {
            RelativePath = value;
        }

        private void AddType(string value)
        {
            Type = value;
        }

        private void AddGroup(string value)
        {
            Group = value;
        }

        private void AddVersion(string value)
        {
            int version;
            if (int.TryParse(value, out version))
                Version = version;
        }
    }
}