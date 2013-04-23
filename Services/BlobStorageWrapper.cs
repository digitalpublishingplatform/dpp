using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Text;
using DigitalPublishingPlatform.Helpers;

namespace DigitalPublishingPlatform.Services
{
    public class BlobStorageWrapper : IBlobStorageWrapper {
        private CloudBlobClient _blobClient;
        private readonly IConfig _config;
        private const string CloudStorageAccountConn = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";
        private CloudBlobClient Client {
            get {
                if (_blobClient == null) {
                    if (_config.BlobStorageAccount.IsNullOrEmpty())
                    {
                        throw new Exception(String.Format("Blob storage account value is empty."));
                    }
                    if (_config.BlobStorageKey.IsNullOrEmpty())
                    {
                        throw new Exception(String.Format("Blob storage key value is empty."));
                    }

                    try {                        
                        _blobClient = CloudStorageAccount.Parse(String.Format(CloudStorageAccountConn, _config.BlobStorageAccount, _config.BlobStorageKey))
                                            .CreateCloudBlobClient();
                    }
                    catch (Exception) {
                        throw new Exception("Application can't connet to azure blob, please check your setting for blob storage key & value, ");
                    }
                }
                return _blobClient;
            }
        }

        public BlobStorageWrapper(IConfig config) {
            _config = config;                                    
        }

        public CloudBlobContainer GetContainerReference(string containerAddress) {
            return Client.GetContainerReference(containerAddress);
        }

        public void CreateIfNotExist(CloudBlobContainer cloudBlobContainer, bool isPublic = true) {
            cloudBlobContainer.CreateIfNotExist();
            var permission = cloudBlobContainer.GetPermissions();
            var blobContainerPermissions = new BlobContainerPermissions {PublicAccess = isPublic ? BlobContainerPublicAccessType.Container : BlobContainerPublicAccessType.Off};
            cloudBlobContainer.SetPermissions(blobContainerPermissions);
        }

        public void UploadText(CloudBlobContainer cloudBlobContainer, string blobName, string blobText) {
            var blob = Client.GetBlobReference(string.Concat(cloudBlobContainer.Name, "/", blobName));
            blob.Attributes.Properties.ContentType = "text/plain";
            blob.UploadText(blobText);
        }

        public void UploadByteArray(CloudBlobContainer cloudBlobContainer, string blobName, byte[] content, string contentType) {
            var blob = Client.GetBlobReference(string.Concat(cloudBlobContainer.Name, "/", blobName));
            blob.Attributes.Properties.ContentType = contentType;
            blob.UploadByteArray(content);
        }

        public string DownloadText(CloudBlobContainer cloudBlobContainer, string blobName) {
            return cloudBlobContainer.GetBlobReference(blobName).DownloadText();
        }

        public byte[] DownloadByteArray(CloudBlobContainer cloudBlobContainer, string blobName) {
            return cloudBlobContainer.GetBlobReference(blobName).DownloadByteArray();
        }

        public bool DeleteIfExists(CloudBlobContainer cloudBlobContainer, string blobName) {
            return cloudBlobContainer.GetBlobReference(blobName).DeleteIfExists();
        }

        public bool Rename(CloudBlobContainer cloudBlobContainer, string oldBlobName, string newBlobName) {
            var oldCloudBlob = cloudBlobContainer.GetBlobReference(oldBlobName);
            cloudBlobContainer.GetBlobReference(newBlobName).CopyFromBlob(oldCloudBlob);
            return oldCloudBlob.DeleteIfExists();
        }

        public string GetBaseUri() {
            return Client.BaseUri.ToString();
        }

        public void PutChunk(CloudBlobContainer container, string filename, int chunk, Stream inputStream) {            
            var cloudBlockBlob = container.GetBlockBlobReference(filename);
            var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes("chk_" + chunk.ToString("D8")));
            cloudBlockBlob.PutBlock(blockId, inputStream, null);            
        }
    }
}