using Microsoft.Extensions.Logging;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System;

namespace dev_tumbnail_generator
{
    public static class ImageHelper
    {
        public static void ProcessImage(Stream inputImageStream, string folder, string subfolder, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function processed blob\n Name:{name} \n  Input Size: {inputImageStream.Length} Bytes");

            using (var outputImageStream = new MemoryStream())
            {
                // Image processing logic here
                using (var image = Image.Load(inputImageStream))
                {
                    // Create a thumbnail
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(100, 100),
                        Mode = ResizeMode.Max
                    }));

                    // Save the processed image to the outputImageStream
                    image.Save(outputImageStream, new JpegEncoder());
                }

                outputImageStream.Position = 0; // Reset position
                UploadImageToBlobStorageAsync(outputImageStream, folder, subfolder, name).Wait();

                log.LogInformation($"C# Blob trigger function processed blob\n Name:{name} \n  Output Size: {outputImageStream.Length} Bytes");
            }
        }

        private static async Task UploadImageToBlobStorageAsync(Stream imageStream, string folder, string subfolder, string name)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage"); // Using the connection string from environment variables

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("archive");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{folder}/{subfolder}/image/result/{name}");

            imageStream.Position = 0;
            await blockBlob.UploadFromStreamAsync(imageStream);
        }
    }


}
