using Orchard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Services
{
    public interface IBlobStorageService : IDependency
    {
        bool StoreBlobData(string containerName, string blobName, string blobText, bool isPublic = true);
        bool StoreBlobData(string containerName, string blobName, byte[] blobData, string contentType, bool isPublic = true);
        bool GetBlobData(string containerName, string blobName, out string blobText);
        bool GetBlobData(string containerName, string blobName, out byte[] blobData);
        bool DeleteBlob(string containerName, string blobName);
        bool RenameBlob(string containerName, string oldBlobName, string newBlobName);
        string GetFullUrl(string containerName, string name);
        string GetContainerNameFromUrl(string url);
        string GetFileNameFromUrl(string url);
        bool UploadChunk(string filename, int chunk, int chunksLength, Stream inputStream, string token);
        void CopyBetweenContainers(string filename, string oldContainerName, string newContainerName, out long filesize);
    }
   
}