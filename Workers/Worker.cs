using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace netcoreCdn
{
    internal class UploadFilesWorker : IWorker
    {
        const string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1";

        private IEnumerable<CdnFile> GetListOfFiles()
        {
            var dirInfo = new DirectoryInfo(@"Files/");
            return dirInfo.GetFiles().Select(fInfo => new CdnFile
            {
                FullName = fInfo.FullName,
                Name = fInfo.Name
            });
        }

        private async Task<CloudBlobContainer> GetCdnContainer()
        {
            // Retrieve storage account information from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Create a reference to the container
            const string ContainerName = "cdn-images";
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);
            if (!await container.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Cannot create container");

            }
            return container;
        }

        public async Task DoWork()
        {
            var container = await GetCdnContainer();
            var listOfFIles = GetListOfFiles();

            foreach (var cndFile in listOfFIles)
            {
                var fileNameInCdn = $"{DateTime.UtcNow.Ticks}-{cndFile.Name}";
                var blobReference = container.GetBlockBlobReference(fileNameInCdn);
                await blobReference.UploadFromFileAsync(cndFile.FullName);
                Console.WriteLine($"Uploading file {fileNameInCdn}...");
            }
        }
    }
}