using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class IndexResource : TextResource
    {
        private readonly ResourceManager _resourceManager;

        internal IndexResource(ResourceManager resourceManager, string indexFileName)
            : base(resourceManager, GetResourceInfo(indexFileName), GetResourceInfo(indexFileName))
        {
            _resourceManager = resourceManager;
        }

        internal async override Task Load()
        {
            if (!_resourceManager.HasInternetConnection)
                return;

        }

        private static ResourceInfo GetResourceInfo(string indexFileName)
        {
            return new ResourceInfo(indexFileName, new Uri(indexFileName), "index", 0);
        }

        internal static void Write(IEnumerable<ResourceInfo> resources)
        {
            JsonConvert.SerializeObject(resources);
        }

        internal static IList<ResourceInfo> Read(string data)
        {
            return JsonConvert.DeserializeObject<List<ResourceInfo>>(data);
        }
    }
}
