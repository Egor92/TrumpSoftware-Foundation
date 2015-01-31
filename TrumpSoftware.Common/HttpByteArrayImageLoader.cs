using System;
using System.Threading.Tasks;

namespace TrumpSoftware.Common
{
    public class HttpByteArrayImageLoader : HttpByteArrayLoader
    {
        public override async Task<byte[]> Load(Uri uri)
        {
            var httpResponseMessage = await HttpClient.GetAsync(uri);
            if (!httpResponseMessage.Content.Headers.ContentType.MediaType.StartsWith("image"))
                throw new Exception("Content is not image");
            return await httpResponseMessage.Content.ReadAsByteArrayAsync();
        }
    }
}
