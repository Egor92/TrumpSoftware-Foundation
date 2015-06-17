using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TrumpSoftware.Common.Interfaces;

namespace TrumpSoftware.Common
{
    public class HttpByteArrayLoader : ILoader<byte[]>, ICancelable
    {
        #region HttpClient

        private HttpClient _httpClient;

        protected HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = GetHttpClient()); }
        }

        protected virtual HttpClient GetHttpClient()
        {
            return new HttpClient();
        }

        #endregion

        public HttpByteArrayLoader()
        {
        }

        public HttpByteArrayLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
