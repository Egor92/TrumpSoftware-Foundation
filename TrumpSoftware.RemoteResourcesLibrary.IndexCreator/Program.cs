using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    class Program
    {
        private const string IndexFileName = @"index.txt";
        private static Uri _selectedDirectoryUri = HeliohostFtpResponser.HelioHostUri;
        private static readonly FtpResponser FtpResponser = new HeliohostFtpResponser();

        public static void Main(string[] args)
        {
            WriteHead();
            SelectResourceFolder();
            Menu();
        }

        private static void PrintDelimeter()
        {
            Console.WriteLine("===================================================================");
        }

        private static void WriteHead()
        {
            Console.WriteLine("===================================================================");
            Console.Write("========================== ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("INDEX CREATOR");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ==========================");
        }

        private static void SelectResourceFolder()
        {
            while (true)
            {
                PrintDelimeter();
                PrintCurrentDirectory();
                Console.WriteLine("Select resource folder:");
                Console.Write("waiting for ftp server....\r");
                var ftpItems = FtpResponser.GetItems(_selectedDirectoryUri);
                var directories = ftpItems.Where(x => x.IsDirectory).ToList();
                Console.Write("                          \r");
                if (directories.Count == 0)
                {
                    Console.WriteLine("Directory hasn't subdirectories!");
                    return;
                }
                for (int i = 0; i < directories.Count; i++)
                    Console.WriteLine("{0}) {1}", i, directories[i].Name);
                Console.WriteLine("Input directory number or \"OK\" to apply");
                string line;
                int index;
                do
                {
                    line = Console.ReadLine();
                    if (line.Trim().ToUpper() == "OK")
                        return;
                } while (!int.TryParse(line, out index) || index < 0 || index >= directories.Count);
                _selectedDirectoryUri = directories[index].Uri;
            }
        }

        private static void PrintCurrentDirectory()
        {
            Console.WriteLine("Current resource folder is ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(_selectedDirectoryUri);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void Menu()
        {
            while (true)
            {
                PrintDelimeter();
                PrintCurrentDirectory();
                Console.WriteLine("Select command:");
                Console.WriteLine("1) Change resource folder");
                Console.WriteLine("2) Create index file");
                Console.WriteLine("3) Create default *.info files for each resource files");
                Console.WriteLine("0) Exit");
                var keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.KeyChar)
                {
                    case '1':
                        SelectResourceFolder();
                        break;
                    case '2':
                        CreateIndexFile();
                        break;
                    case '3':
                        CreateInfoFiles();
                        break;
                    case '0':
                        return;
                    default:
                        continue;
                }
            }
        }

        private static void CreateInfoFiles()
        {
            PrintDelimeter();
            var createdInfoFilesCount = 0;
            CreateInfoFiles(_selectedDirectoryUri, ref createdInfoFilesCount);
            Console.WriteLine("Created {0} info files", createdInfoFilesCount);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void CreateInfoFiles(Uri directoryUri, ref int createdInfoFilesCount)
        {
            Console.Write("...\r");
            var ftpItems = FtpResponser.GetItems(directoryUri);
            Console.Write("   \r");
            var ftpDirectories = ftpItems.Where(x => x.IsDirectory && !x.EndsWithDots);
            var ftpFiles = ftpItems.Where(x => x.IsFile && !x.IsIndexFile);
            foreach (var ftpFile in ftpFiles)
            {
                var extension = Path.GetExtension(ftpFile.Name);
                if (extension == ".info")
                    continue;
                var infoFtpFileName = string.Concat(ftpFile.Name, ".info");
                var infoFtpFile = ftpFiles.FirstOrDefault(x => x.Name == infoFtpFileName);
                if (infoFtpFile == null)
                {
                    var ftpFileUri = directoryUri.Combine(ftpFile.Name);
                    var ftpFileRelativePathLength = ftpFileUri.AbsoluteUri.Length - _selectedDirectoryUri.AbsoluteUri.Length - 1;
                    var ftpFileRelativePath = ftpFileUri.AbsoluteUri.Substring(_selectedDirectoryUri.AbsoluteUri.Length + 1, ftpFileRelativePathLength);
                    var infoFtpFileTemplate = GetDefaultInfoFtpFile(ftpFileRelativePath);
                    var ftpInfoFileUri = new Uri(string.Format("{0}.info", ftpFileUri.AbsoluteUri));
                    FtpResponser.CreateFile(ftpInfoFileUri, infoFtpFileTemplate);
                    Console.WriteLine(ftpFileRelativePath);
                    createdInfoFilesCount++;
                }
            }
            foreach (var ftpDirectory in ftpDirectories)
            {
                var subdirectoryUri = directoryUri.Combine(ftpDirectory.Name);
                CreateInfoFiles(subdirectoryUri, ref createdInfoFilesCount);
            }
        }

        private static string GetDefaultInfoFtpFile(string relativePath)
        {
            var resourceInfo = new ResourceInfo(relativePath);
            return JsonConvert.SerializeObject(resourceInfo);
        }

        private static void CreateIndexFile()
        {
            PrintDelimeter();
            var resourceInfos = new List<ResourceInfo>();
            CollectInfoFiles(_selectedDirectoryUri, resourceInfos);
            var indexContent = JsonConvert.SerializeObject(resourceInfos);
            var indexUri = _selectedDirectoryUri.Combine(IndexFileName);
            FtpResponser.CreateFile(indexUri, indexContent);
            Console.WriteLine("index.txt file was created");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void CollectInfoFiles(Uri directoryUri, IList<ResourceInfo> resourceInfos)
        {
            Console.Write("...\r");
            var ftpItems = FtpResponser.GetItems(directoryUri);
            Console.Write("   \r");
            var ftpDirectories = ftpItems.Where(x => x.IsDirectory && !x.EndsWithDots);
            var ftpFiles = ftpItems.Where(x => x.IsFile && !x.IsIndexFile);
            foreach (var ftpFile in ftpFiles)
            {
                var extension = Path.GetExtension(ftpFile.Name);
                if (extension != ".info")
                    continue;
                var data = FtpResponser.ReadFileAsText(ftpFile.Uri);
                var resourceInfo = JsonConvert.DeserializeObject<ResourceInfo>(data);
                if (resourceInfo != null)
                    resourceInfos.Add(resourceInfo);
            }
            foreach (var ftpDirectory in ftpDirectories)
            {
                var subdirectoryUri = directoryUri.Combine(ftpDirectory.Name);
                CollectInfoFiles(subdirectoryUri, resourceInfos);
            }
        }
    }
}
