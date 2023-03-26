using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CheckFhoto_CognitiveService.Models
{
    public class ClassService
    {
        private static string connection = "DefaultEndpointsProtocol=https;AccountName=storageoliinyk;AccountKey=OcLF9AD9fyF34OA0nYU90JH1jt8quMCrksP6ywJmZOd3uatcYPhcFnFfz+WIEbclqn8EDQqe+X4U+AStZEtZVg==;EndpointSuffix=core.windows.net";
        private static BlobServiceClient blobServiceClient;


        public static async Task DownloadFile(string fileName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("home");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);


            using (MemoryStream stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);
                byte[] bytes = stream.ToArray();

                await File.WriteAllBytesAsync($"wwwroot/img/{fileName}", bytes);
            }
        }

        public static async Task UploadFile(IFormFile file)
        {
            blobServiceClient = new BlobServiceClient(connection);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("home");

            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }
        }




        public static async Task<bool> TryCreateBlobContainer(string containerName)
        {
            try
            {
                blobServiceClient = new BlobServiceClient(connection);

                BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }

        }




    }
}
