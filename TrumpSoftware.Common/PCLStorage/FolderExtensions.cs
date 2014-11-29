using System.Threading;
using System.Threading.Tasks;
using PCLStorage;

namespace TrumpSoftware.Common.PCLStorage
{
    public static class FolderExtensions
    {
        public static async Task CopyFolderAsync(this IFolder copySource, IFolder targetFolder, CreationCollisionOption collisionOption = CreationCollisionOption.OpenIfExists)
        {
            var copiedFolder = await targetFolder.CreateFolderAsync(copySource.Name, collisionOption, CancellationToken.None);
            await copySource.CopyItemsAsync(copiedFolder, collisionOption);
        }

        public static async Task CopyItemsAsync(this IFolder sourceFolder, IFolder targetFolder, CreationCollisionOption collisionOption = CreationCollisionOption.OpenIfExists)
        {
            var files = await sourceFolder.GetFilesAsync();
            foreach (var file in files)
                await file.CopyFileAsync(targetFolder);

            var folders = await sourceFolder.GetFoldersAsync();
            foreach (var folder in folders)
            {
                var copiedFolder = await targetFolder.CreateFolderAsync(folder.Name, collisionOption, CancellationToken.None);
                await folder.CopyItemsAsync(copiedFolder, collisionOption);
            }
        }
    }
}