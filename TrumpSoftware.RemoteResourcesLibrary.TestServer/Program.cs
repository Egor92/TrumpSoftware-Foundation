using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrumpSoftware.RemoteResourcesLibrary.TestServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string rootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\");
            Console.WriteLine("Root folder: \n{0}\n", rootDir);
            Console.WriteLine("Started on: http://localhost:5050/\n");
            MyWebServer server = new MyWebServer(rootDir, 5050);
        }
    }
}