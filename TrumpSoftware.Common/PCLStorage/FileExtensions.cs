using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PCLStorage;

namespace TrumpSoftware.Common.PCLStorage
{
    public static class FileExtensions
    {
        public static async Task CopyFileAsync(this IFile copySource, IFile targetFile)
        {
            if (copySource == null)
                throw new ArgumentNullException("copySource");
            if (targetFile == null)
                throw new ArgumentNullException("targetFile");
            using (var streamToWrite = await targetFile.OpenAsync(FileAccess.ReadAndWrite))
            {    
                using (var streamToRead = await copySource.OpenAsync(FileAccess.Read))
                {
                    await streamToRead.CopyToAsync(streamToWrite);
                }
            }
        }

        public static async Task CopyFileAsync(this IFile copySource, IFolder targetFolder, CreationCollisionOption collisionOption = CreationCollisionOption.ReplaceExisting)
        {
            if (copySource == null)
                throw new ArgumentNullException("copySource");
            if (targetFolder == null)
                throw new ArgumentNullException("targetFolder");
            var targetFile = await targetFolder.CreateFileAsync(copySource.Name, collisionOption, CancellationToken.None);
            await copySource.CopyFileAsync(targetFile);
        }

        public static async Task CopyFileAsync(this IFile copySource, string targetFilePath)
        {
            if (copySource == null)
                throw new ArgumentNullException("copySource");
            if (targetFilePath == null)
                throw new ArgumentNullException("targetFilePath");
            var targetFile = await PathHelper.GetOrCreateFileAsync(targetFilePath);
            await copySource.CopyFileAsync(targetFile);
        }

        public static async Task<byte[]> ReadAsByteArrayAsync(this IFile file)
        {
            using (var fileStream = await file.OpenAsync(FileAccess.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}