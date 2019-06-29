using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Paku.Models.Config;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Paku.Models
{
    /// <summary>
    /// # AzureBlobPakuStrategy
    /// 
    /// Removes files by uploading them to Azure blob storage.
    /// </summary>
    [CommandAlias("azure")]
    [Description("Uploads files to Azure blob storage and then deletes them. Parameter is simply the path to a JSON file with Azure configuration values.")]
    public class AzureBlobPakuStrategy : IPakuStrategy
    {
        /// <summary>
        /// ## Eat
        /// 
        /// Eats files by uploading them to Azure blob storage and then deleting them locally.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="files"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public PakuResult Eat(DirectoryInfo dir, IList<VirtualFileInfo> files, string parameters)
        {
            PakuResult result = new PakuResult();

            try
            {
                AzurePakuConfig config;

                try
                {
                    string json = File.ReadAllText(parameters);
                    config = JsonConvert.DeserializeObject<AzurePakuConfig>(json);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("parameters should be a valid path to an Azure configuration JSON file. JSON keys: ConnectionString, Container.", ex);
                }

                // upload the files
                Upload(config.ConnectionString, config.Container, files);

                // remove all uploaded files
                foreach (VirtualFileInfo vfi in files)
                {
                    FileInfo fi = vfi.ToFileInfo();

                    if (fi.Exists)
                    {
                        fi.Delete();
                        result.RemovedFiles.Add(vfi);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        /// <summary>
        /// ## Upload
        /// 
        /// Uploads the specified files to Azure blob storage.
        /// </summary>
        /// <param name="connectionString">Azure storage account connection string.</param>
        /// <param name="containerName">Name of the container to uploaded to (will be created at the private access level if non-existent).</param>
        /// <param name="files">Files to upload.</param>
        public void Upload(string connectionString, string containerName, IList<VirtualFileInfo> files)
        {
            CloudStorageAccount storageAccount = null;

            if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                // create the blob container if it doesn't already exist
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
                blobContainer.CreateIfNotExists();
                blobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Off });

                // upload each file
                foreach (VirtualFileInfo fi in files)
                {
                    // get reference to blob address then upload
                    CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fi.Name);
                    blockBlob.UploadFromFile(fi.FullName);
                }
            }
        }
    }
}
