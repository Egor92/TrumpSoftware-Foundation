using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrumpSoftware.Common
{
    public class HttpByteArrayImageLoader : HttpByteArrayLoader
    {
        private readonly bool _throwExceptionOnFail;

        public HttpByteArrayImageLoader(bool throwExceptionOnFail = true)
            : this(null, throwExceptionOnFail)
        {
        }

        public HttpByteArrayImageLoader(HttpClient httpClient, bool throwExceptionOnFail = true)
            : base(httpClient)
        {
            _throwExceptionOnFail = throwExceptionOnFail;
        }

        public override async Task<byte[]> Load(Uri uri)
        {
            var httpResponseMessage = await HttpClient.GetAsync(uri);
            if (httpResponseMessage.Content.Headers.ContentType.MediaType.StartsWith("image"))
                return await httpResponseMessage.Content.ReadAsByteArrayAsync();
            if (_throwExceptionOnFail)
                throw new Exception("Content is not image");
            return null;
        }
    }
}
