using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace TrumpSoftware.RemoteResourcesLibrary.Test
{
    public class HttpServer : IDisposable
    {
        private const uint BufferSize = 8192;

        private static StorageFolder _localFolder;

        private readonly StreamSocketListener _listener;

        public HttpServer(StorageFolder localFolder, int port)
        {
            _localFolder = localFolder;
            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
            _listener.BindServiceNameAsync(port.ToString());
        }

        public void Dispose()
        {
            _listener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            var request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                var data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');

                if (requestParts[0] == "GET")
                    await WriteResponseAsync(requestParts[1], output);
                else
                    throw new InvalidDataException("HTTP method not supported: "
                                                   + requestParts[0]);
            }
        }

        private async Task WriteResponseAsync(string path, IOutputStream os)
        {
            path = path.Replace('/', '\\');
            if (path.StartsWith("\\"))
                path = path.Substring(1);
            using (Stream resp = os.AsStreamForWrite())
            {
                bool exists = true;
                try
                {
                    using (Stream fs = await _localFolder.OpenStreamForReadAsync(path))
                    {
                        string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                                      "Content-Length: {0}\r\n" +
                                                      "Connection: close\r\n\r\n",
                            fs.Length);
                        byte[] headerArray = Encoding.UTF8.GetBytes(header);
                        await resp.WriteAsync(headerArray, 0, headerArray.Length);
                        await fs.CopyToAsync(resp);
                    }
                }
                catch (Exception)
                {
                    exists = false;
                }

                if (!exists)
                {
                    byte[] headerArray = Encoding.UTF8.GetBytes(
                        "HTTP/1.1 404 Not Found\r\n" +
                        "Content-Length:0\r\n" +
                        "Connection: close\r\n\r\n");
                    await resp.WriteAsync(headerArray, 0, headerArray.Length);
                }

                await resp.FlushAsync();
            }
        }
    }
}