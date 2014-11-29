using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;

namespace TrumpSoftware.Common
{
    public static class PathHelper
    {
        private static readonly string[] Slashes = { "/", "\\" };

        public static async Task<IFile> GetOrCreateFileAsync(string path)
        {
            var targetFile = await FileSystem.Current.GetFileFromPathAsync(path);
            if (targetFile != null)
                return targetFile;
            var parentFolderPath = GetParentFolderPath(path);
            var folder = await GetOrCreateFolderAsync(parentFolderPath);
            var fileName = Path.GetFileName(path);
            return await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }

        public static async Task<IFolder> GetOrCreateFolderAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Invalid path", "path");
            var targetFolder = await FileSystem.Current.GetFolderFromPathAsync(path);
            if (targetFolder != null)
                return targetFolder;
            var parentPath = GetParentFolderPath(path);
            var parentFolder = await GetOrCreateFolderAsync(parentPath);
            var directoryName = GetItemName(path);
            return await parentFolder.CreateFolderAsync(directoryName, CreationCollisionOption.ReplaceExisting);
        }

        public static string GetParentFolderPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (Slashes.Any(path.EndsWith))
                path = path.Substring(0, path.Length - 1);
            var fileName = GetItemName(path);
            if (fileName == null)
                throw new ArgumentException("File name is null", "path");
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is empty", "path");
            return path.Substring(0, path.Length - fileName.Length);
        }

        public static string GetItemName(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (Slashes.Any(path.EndsWith))
                path = path.Substring(0, path.Length - 1);
            return path.Split(Slashes, StringSplitOptions.None).LastOrDefault();
        }
    }
}