using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DigitalPublishingPlatform.Helpers;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;

namespace DigitalPublishingPlatform.Services
{
    public class BlobStorageService: IBlobStorageService
    {
        private CloudBlobContainer _blobContainer;
        private readonly IBlobStorageWrapper _blobStorageWrapper;
        public BlobStorageService(IBlobStorageWrapper blobStorageWrapper)
        {
            _blobStorageWrapper = blobStorageWrapper;
        }

        private CloudBlobContainer GetBlobContainer(string containerName, bool isPublic = true)
        {
            HandleRestCall(() =>
            {
                _blobContainer = _blobStorageWrapper.GetContainerReference(containerName);
                _blobStorageWrapper.CreateIfNotExist(_blobContainer, isPublic);
            });
            return _blobContainer;
        }

        public bool StoreBlobData(string containerName, string blobName, string blobText, bool isPublic = true)
        {
            return HandleRestCall(() =>
            {
                var container = GetBlobContainer(containerName, isPublic);
                _blobStorageWrapper.UploadText(container, blobName, blobText);
            });
        }

        public bool StoreBlobData(string containerName, string blobName, byte[] blobData, string contentType, bool isPublic = true)
        {
            return HandleRestCall(() =>
            {
                var container = GetBlobContainer(containerName, isPublic);
                _blobStorageWrapper.UploadByteArray(container, blobName, blobData, contentType);
            });
        }

        public bool GetBlobData(string containerName, string blobName, out string blobText)
        {
            var returnBlobText = String.Empty;
            var result = HandleRestCall(() =>
            {
                var container = GetBlobContainer(containerName);
                returnBlobText = _blobStorageWrapper.DownloadText(container, blobName);
                
            });
            blobText = returnBlobText;
            return result;
        }

        public bool GetBlobData(string containerName, string blobName, out byte[] blobData)
        {
            var bytes = new byte[0];
            var result = HandleRestCall(() =>
            {
                var container = GetBlobContainer(containerName);
                bytes = _blobStorageWrapper.DownloadByteArray(container, blobName);
            });
            blobData = bytes;
            return result;
        }

        public bool DeleteBlob(string containerName, string blobName)
        {
            var deletedOkay = false;
            var result = HandleRestCall(() =>
            {
                var container = GetBlobContainer(containerName);
                deletedOkay = _blobStorageWrapper.DeleteIfExists(container, blobName);
            });
            return result && deletedOkay;
        }

        public bool RenameBlob(string containerName, string oldBlobName, string newBlobName)
        {
            var renamedOkay = false;
            var result = HandleRestCall(() =>
            {
                var container = GetBlobContainer(containerName);
                renamedOkay = _blobStorageWrapper.Rename(container, oldBlobName, newBlobName);
            });
            return result && renamedOkay;
        }

        public bool UploadChunk(string filename, int chunk, int chunksLength, Stream inputStream, string token)
        {
            return HandleRestCall(() => {
                Debug.WriteLine("Begin uploading chunk {0}", chunk);
                Debug.WriteLine("Token: {0}", token);
                Debug.WriteLine("Stream length = {0}", inputStream.Length);
                var container = GetBlobContainer(token);
                
                _blobStorageWrapper.PutChunk(container, filename, chunk, inputStream);

                if (chunk != chunksLength - 1) return;
                var blockNames = new List<string>();
                for (var i = 0; i <= chunk; i++)
                {
                    blockNames.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("chk_" + i.ToString("D8"))));
                }
                var cloudBlockBlob = container.GetBlockBlobReference(filename);
                cloudBlockBlob.Properties.ContentType = filename.MimeType();
                cloudBlockBlob.PutBlockList(blockNames);
                Debug.WriteLine("Ending chunk {0}", chunk);
                
             });
        }

        public void CopyBetweenContainers(string filename, string oldContainerName, string newContainerName, out long fileSize) {            
                var oldContainer = GetBlobContainer(oldContainerName);
                var newContainer = GetBlobContainer(newContainerName);
                var oldBlob = oldContainer.GetBlobReference(filename);
                var newBlob = newContainer.GetBlobReference(filename);
                newBlob.DeleteIfExists();
                newBlob.CopyFromBlob(oldBlob);
                newBlob.FetchAttributes();
                fileSize = newBlob.Properties.Length;
                newBlob.Properties.ContentType = filename.MimeType();
                newBlob.SetProperties();                
                newBlob.SetMetadata();
                oldContainer.Delete();
            
        }

        public string GetFullUrl(string containerName, string blobName)
        {
            return string.Format("{0}{1}/{2}", _blobStorageWrapper.GetBaseUri().MustEndWith("/"), containerName, blobName);
        }

        public string GetContainerNameFromUrl(string url)
        {
            var parts = url.Split('/');
            return parts[parts.Length - 2];
        }

        public string GetFileNameFromUrl(string url)
        {
            var parts = url.Split('/');
            return parts[parts.Length - 1];
        }

        public bool HandleRestCall(Action action)
        {            
            try
            {
                action();
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }
    }
}