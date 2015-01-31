using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrumpSoftware.Common
{
    public class HttpByteArrayLoader : ILoader<byte[]>, ICancelable
    {
        private HttpClient _httpClient;

        protected HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = new HttpClient()); }
        }

        public virtual async Task<byte[]> Load(Uri uri)
        {
            return await HttpClient.GetByteArrayAsync(uri);
        }

        public void Cancel()
        {
            HttpClient.CancelPendingRequests();
        }
    }
}
