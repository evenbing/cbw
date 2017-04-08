using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    public class BlobDataProvider
    {
        public CloudBlobContainer Container { get; set; }

        public BlobDataProvider(string connectionString)
        {
            string containerName = "cbwdict";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
            blobContainer.CreateIfNotExists();
            this.Container = blobContainer;
        }

        public virtual void Upload(string blobName, Stream content)
        {
            CloudBlockBlob blockBlob = this.Container.GetBlockBlobReference(blobName);
            blockBlob.UploadFromStream(content);
        }

        public virtual void Download(string blobName, Stream content)
        {
            CloudBlockBlob blockBlob = this.Container.GetBlockBlobReference(blobName);
            blockBlob.DownloadToStream(content);
        }
    }
}
