using System;
using System.IO;
using System.Threading.Tasks;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public interface IResource
    {
        Task<Stream> GetStreamAsync();

        Uri GetUri();
    }
}