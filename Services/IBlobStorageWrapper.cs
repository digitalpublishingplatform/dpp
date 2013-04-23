
using System.IO;
using Microsoft.WindowsAzure.StorageClient;
using Orchard;

namespace DigitalPublishingPlatform.Services
{
    public interface IBlobStorageWrapper : IDependency
    {
        CloudBlobContainer GetContainerReference(string containerAddress);
        void CreateIfNotExist(CloudBlobContainer cloudBlobContainer, bool isPublic = true);
        void UploadText(CloudBlobContainer cloudBlobContainer, string blobName, string content);
        void UploadByteArray(CloudBlobContainer cloudBlobContainer, string blobName, byte[] content, string contentType);
        string DownloadText(CloudBlobContainer cloudBlobContainer, string blobName);
        byte[] DownloadByteArray(CloudBlobContainer cloudBlobContainer, string blobName);
        bool DeleteIfExists(CloudBlobContainer cloudBlobContainer, string blobName);
        bool Rename(CloudBlobContainer cloudBlobContainer, string oldBlobName, string newBlobName);
        string GetBaseUri();
        void PutChunk(CloudBlobContainer container, string filename, int chunk, Stream inputStream);
    }
}
