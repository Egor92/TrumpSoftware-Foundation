using System.IO;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class StringResourceConverter : IResourceConverter<string>
    {
        string IResourceConverter<string>.Convert(IResource resource)
        {
            var stream = resource.GetStreamAsync().Result;
            string result;
            using (var streamReader = new StreamReader(stream))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}