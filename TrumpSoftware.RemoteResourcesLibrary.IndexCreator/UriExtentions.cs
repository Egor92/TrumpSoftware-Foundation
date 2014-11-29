using System;
using System.IO;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public static class UriExtentions
    {
        public static Uri Combine(this Uri uri, string relativePath)
        {
            var path = Path.Combine(uri.AbsoluteUri, relativePath);
            return new Uri(path);
        }
    }
}