using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System;
using System.IO;

namespace TimeLapse.Azure
{
    public class UploadFiles
    {
        public static void Upload(string path, string uploadFile, string containerName)
        {

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Microsoft.Azure.CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(uploadFile);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(Path.Combine(path, uploadFile)))
            {
                blockBlob.UploadFromStream(fileStream);
            }


        }
    }
}
