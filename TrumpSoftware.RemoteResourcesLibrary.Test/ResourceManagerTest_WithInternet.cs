using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using PCLStorage;
using TrumpSoftware.Common;
using TrumpSoftware.Common.PCLStorage;
using CreationCollisionOption = PCLStorage.CreationCollisionOption;

namespace TrumpSoftware.RemoteResourcesLibrary.Test
{
    [TestClass]
    public class ResourceManagerTest_WithInternet
    {
        private const string LocalContent = @"local";
        private const string RemoteContent = @"remote";
        private static ResourceManager _resourceManager;
        private static HttpServer _httpServer;
        private static IFolder _compiledResourceFolder;
        private static IFolder _localResourceFolder;
        private static IFolder _remoteResourceFolder;

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
        }

        [TestInitialize]
        public async Task Initialize()
        {
            var compiledStorageResourceFolder = await FileSystem.Current.GetFolderFromPathAsync(string.Format("{0}\\Resources\\", Package.Current.InstalledLocation.Path));
            _compiledResourceFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("Resources", CreationCollisionOption.OpenIfExists);
            await compiledStorageResourceFolder.CopyItemsAsync(_compiledResourceFolder);

            _localResourceFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("Local", CreationCollisionOption.OpenIfExists);

            var remoteStorageResourceFolder = await FileSystem.Current.GetFolderFromPathAsync(string.Format("{0}\\Remote\\", Package.Current.InstalledLocation.Path));
            _remoteResourceFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("Remote", CreationCollisionOption.OpenIfExists);
            await remoteStorageResourceFolder.CopyItemsAsync(_remoteResourceFolder);

            var compiledResourceFolderUri = new Uri(string.Format("{0}\\", _compiledResourceFolder.Path));
            var localResourcesFolderUri = new Uri(string.Format("{0}\\", _localResourceFolder.Path));
            var remoteResourcesFolderUri = new Uri(SpecialPaths.ServerPath);
            _resourceManager = new ResourceManager(compiledResourceFolderUri, localResourcesFolderUri, remoteResourcesFolderUri);

            var remoteResourceStorageFolder = await StorageFolder.GetFolderFromPathAsync(_remoteResourceFolder.Path);
            _httpServer = new HttpServer(remoteResourceStorageFolder, 5050);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _compiledResourceFolder.DeleteAsync();
            _compiledResourceFolder = null;
            await _localResourceFolder.DeleteAsync();
            _localResourceFolder = null;
            await _remoteResourceFolder.DeleteAsync();
            _remoteResourceFolder = null;

            _httpServer.Dispose();
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
        }

        [TestMethod]
        public async Task IfRemoteVersionIsLowerOfLocal_ReturnLocalResourse()
        {
            const string relativePath = "IfRemoteVersionIsLowerOfLocal_ReturnLocalResourse.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Type = "text",
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 0;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetResourceAsync<string>(relativePath);
            Assert.AreEqual(LocalContent, str);
        }

        [TestMethod]
        public async Task IfRemoteVersionEqualsOfLocal_ReturnLocalResourse()
        {
            const string relativePath = "IfRemoteVersionEqualsOfLocal_ReturnLocalResourse.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Type = "text",
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 1;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetResourceAsync<string>(relativePath);
            Assert.AreEqual(LocalContent, str);
        }

        [TestMethod]
        public async Task IfRemoteVersionIsUpperOfLocal_ReturnRemoteResourse()
        {
            const string relativePath = "IfRemoteVersionIsUpperOfLocal_ReturnRemoteResourse.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Type = "text",
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 2;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetResourceAsync<string>(relativePath);
            Assert.AreEqual(RemoteContent, str);
        }

        [TestMethod]
        public async Task CanReturnResourceGroup()
        {
            const string starContent = "star-group";
            const string relativePath1 = "StarFile1.txt";
            var resourceInfo1 = new ResourceInfo
            {
                RelativePath = relativePath1,
                Type = "text",
                Group = "star",
                Version = 1
            };
            await AddFileToRemoteFolderAsync(starContent, resourceInfo1);

            const string relativePath2 = "StarFile2.txt";
            var resourceInfo2 = new ResourceInfo
            {
                RelativePath = relativePath2,
                Type = "text",
                Group = "star",
                Version = 1
            };
            await AddFileToRemoteFolderAsync(starContent, resourceInfo2);

            await _resourceManager.LoadIndexAsync();
            var group = await _resourceManager.GetResourceGroupAsync<string>("star");
            Assert.AreEqual(2, group.Count());
            foreach (var resourceContent in group)
                Assert.AreEqual(starContent, resourceContent);
        }

        [TestMethod]
        public async Task IfRemoteVersionIsUpperOfLocal_LocalVersionWillBeUpdated()
        {
            const string resourceFileName = "IfRemoteVersionIsUpperOfLocal_LocalVersionWillBeUpdated.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Type = "text",
                Version = 1
            };
            await AddFileToRemoteFolderAsync("first content", resourceInfo);
            await _resourceManager.LoadResourceAsync(resourceFileName);

            resourceInfo.Version = 20;
            await AddFileToRemoteFolderAsync("updated content", resourceInfo);
            await _resourceManager.LoadIndexAsync();
            await _resourceManager.LoadResourceAsync(resourceFileName);
            var resInfo = await GetResourceInfoAsync(resourceFileName);

            Assert.AreEqual(20, resInfo.Version);
        }

        [TestMethod]
        public async Task CanReturnNewRemoteResource()
        {
            const string content = "Some content";
            const string resourceFileName = "CanReturnNewRemoteResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Type = "text",
                Version = 1
            };
            await AddFileToRemoteFolderAsync(content, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetResourceAsync<string>(resourceFileName);
            Assert.AreEqual(content, str);
        }

        [TestMethod]
        public async Task IfResourceIsAbsentInRemoteIndex_ReturnNull()
        {
            const string resourceFileName = "IfResourceIsAbsentInRemoteIndex_ReturnNull.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Type = "text",
                Version = 1
            };
            await AddFileToRemoteFolderAsync("Some content", resourceInfo);

            await _resourceManager.LoadIndexAsync();

            await RemoveFileFromRemoteFolderAsync(resourceFileName);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetResourceAsync<string>(resourceFileName);
            Assert.IsNull(str);
        }

        [TestMethod]
        public async Task CanReturnResourceFromSubFolder()
        {
            const string expectedContent = @"You can!";
            string relativePath = Path.Combine("Subfolder1", "Subfolder2", "Subfolder3", "CanReturnResourceFromSubFolder.txt");
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Type = "text",
                Version = 1
            };
            await AddFileToRemoteFolderAsync(expectedContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetResourceAsync<string>(relativePath);
            Assert.AreEqual(expectedContent, str);
        }





        private static async Task<ResourceInfo> GetResourceInfoAsync(string relativePath)
        {
            var indexFile = await _remoteResourceFolder.GetFileAsync("index.txt");
            var indexData = await indexFile.ReadAllTextAsync();
            var resourceInfos = JsonConvert.DeserializeObject<List<ResourceInfo>>(indexData);
            return resourceInfos.SingleOrDefault(x => x.RelativePath == relativePath);
        }

        private static async Task AddFileToCompiledFolderAsync(string content, ResourceInfo resourceInfo)
        {
            await AddFileToFolderAsync(_compiledResourceFolder, content, resourceInfo);
        }

        private static async Task AddFileToRemoteFolderAsync(string content, ResourceInfo resourceInfo)
        {
            await AddFileToFolderAsync(_remoteResourceFolder, content, resourceInfo);
        }

        private static async Task AddFileToFolderAsync(IFolder folder, string content, ResourceInfo resourceInfo)
        {
            var fullFilePath = Path.Combine(folder.Path, resourceInfo.RelativePath);
            var file = await PathHelper.GetOrCreateFileAsync(fullFilePath);
            await file.WriteAllTextAsync(content);
            var indexFile = await folder.GetFileAsync("index.txt");
            var indexData = await indexFile.ReadAllTextAsync();
            var resourceInfos = JsonConvert.DeserializeObject<List<ResourceInfo>>(indexData);
            var neededResourceInfo = resourceInfos.SingleOrDefault(x => x.RelativePath == resourceInfo.RelativePath);
            if (neededResourceInfo != null)
                resourceInfos.Remove(neededResourceInfo);
            resourceInfos.Add(resourceInfo);
            var serializedIndexData = JsonConvert.SerializeObject(resourceInfos);
            await indexFile.WriteAllTextAsync(serializedIndexData);
        }

        private static async Task RemoveFileFromCompiledFolderAsync(string relativePath)
        {
            await RemoveFileFromFolderAsync(_compiledResourceFolder, relativePath);
        }

        private static async Task RemoveFileFromRemoteFolderAsync(string relativePath)
        {
            await RemoveFileFromFolderAsync(_remoteResourceFolder, relativePath);
        }

        private static async Task RemoveFileFromFolderAsync(IFolder folder, string relativePath)
        {
            var fullFilePath = Path.Combine(folder.Path, relativePath);
            var file = await FileSystem.Current.GetFileFromPathAsync(fullFilePath);
            await file.DeleteAsync();

            var indexFile = await folder.GetFileAsync("index.txt");
            var indexData = await indexFile.ReadAllTextAsync();
            var resourceInfos = JsonConvert.DeserializeObject<List<ResourceInfo>>(indexData);
            var neededResourceInfo = resourceInfos.SingleOrDefault(x => x.RelativePath == relativePath);
            if (neededResourceInfo != null)
                resourceInfos.Remove(neededResourceInfo);
            var serializedIndexData = JsonConvert.SerializeObject(resourceInfos);
            await indexFile.WriteAllTextAsync(serializedIndexData);
        }
    }
}
