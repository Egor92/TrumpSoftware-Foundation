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
using TrumpSoftware.Common.PCLStorage;
using TrumpSoftware.Common.Tests;
using CreationCollisionOption = PCLStorage.CreationCollisionOption;

namespace TrumpSoftware.RemoteResourcesLibrary.Test
{
    [TestClass]
    public class ResourceManagerTest
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
            var resourceFolderLocations = new ResourceFolderLocations(compiledResourceFolderUri, localResourcesFolderUri, remoteResourcesFolderUri);
            _resourceManager = new ResourceManager(resourceFolderLocations);

            await EnableInternetAsync();
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

            await DisableInternetAsync();
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
        }



        [TestMethod]
        public async Task CanReturnStringResource()
        {
            const string relativePath = "CanReturnStringResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync("string", resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetStringResourceAsync(relativePath);
            Assert.AreEqual("string", str);
        }

        [TestMethod]
        public async Task CanReturnIntegerResource()
        {
            const int content = 123456789;
            const string relativePath = "CanReturnIntegerResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync(content.ToString(), resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var i = await _resourceManager.GetIntegerResourceAsync(relativePath);
            Assert.AreEqual(content, i);
        }

        [TestMethod]
        public async Task CanReturnDoubleResource()
        {
            const double content = 12345.6789;
            const string relativePath = "CanReturnDoubleResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync(content.ToString(), resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var d = await _resourceManager.GetDoubleResourceAsync(relativePath);
            Assert.AreEqual(content, d);
        }

        [TestMethod]
        public async Task CanReturnUriResource()
        {
            const string relativePath = "CanReturnUriResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync("some content", resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var actualUri = await _resourceManager.GetUriResourceAsync(relativePath);
            var expectedUri = new Uri(Path.Combine(_localResourceFolder.Path, relativePath));
            Assert.AreEqual(expectedUri, actualUri);
        }

        [TestMethod]
        public async Task IfCanNotParseIntegerResource_ThrowException()
        {
            const string content = "2.3";
            const string relativePath = "IfCanNotParseIntegerResource_ThrowException.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync(content, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            await AsyncAssert.ThrowsExceptionAsync<ResourceConvertionException>(async () =>
            {
                await _resourceManager.GetIntegerResourceAsync(relativePath);
            });
        }

        [TestMethod]
        public async Task IfCanNotParseDoubleResource_ThrowException()
        {
            const string content = "Abc";
            const string relativePath = "IfCanNotParseDoubleResource_ThrowException.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync(content, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            await AsyncAssert.ThrowsExceptionAsync<ResourceConvertionException>(async () =>
            {
                await _resourceManager.GetDoubleResourceAsync(relativePath);
            });
        }

        [TestMethod]
        public async Task IfResourceManagerDidNotLoaded_ThrowException()
        {
            const string relativePath = "IfResourceManagerDidNotLoaded_ThrowException.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await AsyncAssert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _resourceManager.GetStringResourceAsync(relativePath);
            }); 
        }

        [TestMethod]
        public async Task IfRemoteVersionIsLowerOfLocal_ReturnLocalResourse()
        {
            const string relativePath = "IfRemoteVersionIsLowerOfLocal_ReturnLocalResourse.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 0;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetStringResourceAsync(relativePath);
            Assert.AreEqual(LocalContent, str);
        }

        [TestMethod]
        public async Task IfRemoteVersionEqualsOfLocal_ReturnLocalResourse()
        {
            const string relativePath = "IfRemoteVersionEqualsOfLocal_ReturnLocalResourse.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 1;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetStringResourceAsync(relativePath);
            Assert.AreEqual(LocalContent, str);
        }

        [TestMethod]
        public async Task IfRemoteVersionIsUpperOfLocal_ReturnRemoteResourse()
        {
            const string relativePath = "IfRemoteVersionIsUpperOfLocal_ReturnRemoteResourse.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 2;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetStringResourceAsync(relativePath);
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
                Group = "star",
                Version = 1
            };
            await AddFileToRemoteFolderAsync(starContent, resourceInfo1);

            const string relativePath2 = "StarFile2.txt";
            var resourceInfo2 = new ResourceInfo
            {
                RelativePath = relativePath2,
                Group = "star",
                Version = 1
            };
            await AddFileToRemoteFolderAsync(starContent, resourceInfo2);

            await _resourceManager.LoadIndexAsync();
            var group = await _resourceManager.GetStringResourceGroupAsync("star");
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
                Version = 1
            };
            await AddFileToRemoteFolderAsync("first content", resourceInfo);
            await _resourceManager.LoadResourceAsync(resourceFileName);

            resourceInfo.Version = 20;
            await AddFileToRemoteFolderAsync("updated content", resourceInfo);
            await _resourceManager.LoadIndexAsync();
            await _resourceManager.LoadResourceAsync(resourceFileName);
            await _resourceManager.SaveIndexAsync();
            var resInfo = await GetLocalResourceInfoAsync(resourceFileName);

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
                Version = 1
            };
            await AddFileToRemoteFolderAsync(content, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetStringResourceAsync(resourceFileName);
            Assert.AreEqual(content, str);
        }

        [TestMethod]
        public async Task IfResourceIsAbsentInRemoteIndex_ThrowException()
        {
            const string resourceFileName = "IfResourceIsAbsentInRemoteIndex_ThrowException.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Version = 1
            };
            await AddFileToRemoteFolderAsync("Some content", resourceInfo);

            await _resourceManager.LoadIndexAsync();

            await RemoveFileFromRemoteFolderAsync(resourceFileName);

            await _resourceManager.LoadIndexAsync();
            await AsyncAssert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await _resourceManager.GetStringResourceAsync(resourceFileName);
            }); 
        }

        [TestMethod]
        public async Task CanReturnResourceFromSubFolder()
        {
            const string expectedContent = @"You can!";
            string relativePath = Path.Combine("Subfolder1", "Subfolder2", "Subfolder3", "CanReturnResourceFromSubFolder.txt");
            var resourceInfo = new ResourceInfo
            {
                RelativePath = relativePath,
                Version = 1
            };
            await AddFileToRemoteFolderAsync(expectedContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            var str = await _resourceManager.GetStringResourceAsync(relativePath);
            Assert.AreEqual(expectedContent, str);
        }

        [TestMethod]
        public async Task IfNoInternetConnetion_ReturnLocalResource()
        {
            await DisableInternetAsync();

            const string resourceFileName = "IfNoInternetConnetion_ReturnLocalResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 2;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();

            var str = await _resourceManager.GetStringResourceAsync(resourceFileName);
            Assert.AreEqual(LocalContent, str);
        }

        [TestMethod]
        public async Task LoadRemoteResourceInfo_ThenDisableInternet_ReturnLocalResource()
        {
            const string resourceFileName = "LoadRemoteResourceInfo_ThenDisableInternet_ReturnLocalResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 2;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();

            await DisableInternetAsync();

            var str = await _resourceManager.GetStringResourceAsync(resourceFileName);
            Assert.AreEqual(LocalContent, str);

            await _resourceManager.SaveIndexAsync();
            var resInfo = await GetLocalResourceInfoAsync(resourceFileName);
            Assert.AreEqual(1, resInfo.Version);
        }

        [TestMethod]
        public async Task PreloadResource_ThenDisableInternet_ReturnRemoteResource()
        {
            const string resourceFileName = "PreloadResource_ThenDisableInternet_ReturnRemoteResource.txt";
            var resourceInfo = new ResourceInfo
            {
                RelativePath = resourceFileName,
                Version = 1
            };
            await AddFileToCompiledFolderAsync(LocalContent, resourceInfo);

            resourceInfo.Version = 2;
            await AddFileToRemoteFolderAsync(RemoteContent, resourceInfo);

            await _resourceManager.LoadIndexAsync();
            await _resourceManager.LoadResourceAsync(resourceFileName);

            await DisableInternetAsync();

            var str = await _resourceManager.GetStringResourceAsync(resourceFileName);
            Assert.AreEqual(RemoteContent, str);

            await _resourceManager.SaveIndexAsync();
            var resInfo = await GetLocalResourceInfoAsync(resourceFileName);
            Assert.AreEqual(2, resInfo.Version);
        }




        private static async Task EnableInternetAsync()
        {
            var remoteResourceStorageFolder = await StorageFolder.GetFolderFromPathAsync(_remoteResourceFolder.Path);
            _httpServer = new HttpServer(remoteResourceStorageFolder, 5050);
        }

        private static async Task DisableInternetAsync()
        {
            if (_httpServer == null)
                return;
            _httpServer.Dispose();
            _httpServer = null;
        }




        private static async Task<ResourceInfo> GetLocalResourceInfoAsync(string relativePath)
        {
            var indexFile = await _localResourceFolder.GetFileAsync("index.txt");
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
