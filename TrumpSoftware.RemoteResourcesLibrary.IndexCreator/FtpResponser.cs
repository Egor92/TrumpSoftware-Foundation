using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    internal abstract class FtpResponser
    {
        private readonly string _ftpUserName;
        private readonly string _ftpPassword;

        public IEnumerable<FtpItem> GetItems(Uri directoryUri)
        {
            var details = GetDirectoryDetails(directoryUri);
            return details.Select(x => new FtpItem(directoryUri, x));
        }

        protected FtpResponser(string ftpUserName, string ftpPassword)
        {
            _ftpUserName = ftpUserName;
            _ftpPassword = ftpPassword;
        }

        public void CreateFile(Uri ftpFileUri, string text)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpFileUri);
            ftpRequest.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            byte[] fileContents = Encoding.UTF8.GetBytes(text);
            ftpRequest.ContentLength = fileContents.Length;

            Stream requestStream = ftpRequest.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();

            response.Close();
        }

        public string ReadFileAsText(Uri fileUri)
        {
            return GetResponseResult(fileUri, WebRequestMethods.Ftp.DownloadFile);
        }

        private IEnumerable<string> GetDirectoryDetails(Uri directoryUri)
        {
            return GetResponseResult(directoryUri, WebRequestMethods.Ftp.ListDirectoryDetails)
                    .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string GetResponseResult(Uri uri, string method)
        {
            string result = string.Empty;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
                ftpRequest.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);
                ftpRequest.Method = method;
                using (var response = (FtpWebResponse) ftpRequest.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                var foregroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = foregroundColor;
                Console.Read();
            }
            return result;
        }
    }
}