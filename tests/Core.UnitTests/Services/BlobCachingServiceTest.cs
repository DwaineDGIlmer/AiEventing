using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace Core.Services.Tests
{
    public sealed class BlobCachingServiceTest
    {
        [Fact]
        public void Constructor_Throws_WhenConfigIsNull()
        {
            var serviceClient = new Mock<BlobServiceClient>().Object;
            Assert.Throws<ArgumentNullException>(() =>
                new BlobCachingService(serviceClient, null!));
        }

        [Fact]
        public void Constructor_Throws_WhenServiceClientIsNull()
        {
            var configMock = new Mock<IOptions<MemoryCacheSettings>>();
            Assert.Throws<ArgumentNullException>(() =>
                new BlobCachingService(null!, configMock.Object));
        }

        [Fact]
        public async Task PutAsync_WOrks()
        {
            var settings = new MemoryCacheSettings
            {
                BlobName = "myblob",
                Container = "mycontainer"
            };
            var configMock = new Mock<IOptions<MemoryCacheSettings>>();
            configMock.Setup(x => x.Value).Returns(settings);

            var blobContainerClientMock = new Mock<BlobContainerClient>();
            blobContainerClientMock.Setup(x => x.CreateIfNotExists(It.IsAny<PublicAccessType>(), null, null, default));
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(new Mock<BlobClient>().Object);

            var serviceClientMock = new Mock<BlobServiceClient>();
            serviceClientMock.Setup(x => x.GetBlobContainerClient("mycontainer"))
                .Returns(blobContainerClientMock.Object);

            var service = new BlobCachingService(serviceClientMock.Object, configMock.Object);
            var results = await service.PutAsync("test", System.Text.Encoding.UTF8.GetBytes("{}"));
            Assert.NotNull(results);
        }

        [Fact]
        public void Constructor_InitializesFieldsCorrectly()
        {
            var settings = new MemoryCacheSettings
            {
                BlobName = "myblob",
                Container = "mycontainer"
            };
            var configMock = new Mock<IOptions<MemoryCacheSettings>>();
            configMock.Setup(x => x.Value).Returns(settings);

            var blobContainerClientMock = new Mock<BlobContainerClient>();
            blobContainerClientMock.Setup(x => x.CreateIfNotExists(It.IsAny<PublicAccessType>(), null, null, default));

            var serviceClientMock = new Mock<BlobServiceClient>();
            serviceClientMock.Setup(x => x.GetBlobContainerClient("mycontainer"))
                .Returns(blobContainerClientMock.Object);

            var service = new BlobCachingService(serviceClientMock.Object, configMock.Object);

            // Use reflection to check private fields
            var prefixField = typeof(BlobCachingService).GetField("_prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.Equal("cache", prefixField?.GetValue(service));
        }
    }
}