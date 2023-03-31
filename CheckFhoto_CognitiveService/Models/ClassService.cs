using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Reflection.Metadata;

namespace CheckFhoto_CognitiveService.Models
{
    public class ClassService
    {
        private static string connection = "DefaultEndpointsProtocol=https;AccountName=storagenewtoday;AccountKey=EsIIDEWtFs1cof9N51eavuB8bbGMx/7I6cbt+110RCJsoKHhuLwlSlP6DokDvuJ33nG2KOEPesnh+ASt/kFwBA==;EndpointSuffix=core.windows.net";
        private static BlobServiceClient blobServiceClient { get; set; }
        private static BlobContainerClient containerClient { get; set; }
        private static BlobClient blobClient { get; set; }
        private static string path = "home";
        public string fileName { get; set; }

        public ClassService()
        {

            blobServiceClient = new BlobServiceClient(connection);
            try
            {
                containerClient = blobServiceClient.CreateBlobContainer(path);
            }
            catch (Exception)
            {
                containerClient = blobServiceClient.GetBlobContainerClient(path);
            }
        }



        public static async Task DownloadFile(string fileName)
        {
            //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(path);
            //BlobClient blobClient = containerClient.GetBlobClient(fileName);
            //await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            //{
            //    Console.WriteLine($"Blob name: {blobItem.Name}");
            //}

            using (MemoryStream stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);
                byte[] bytes = stream.ToArray();

                //await File.WriteAllBytesAsync($"wwwroot/img/{fileName}", bytes);
            }
        }


        public static  async Task<string> GetFile()
        {
           
            containerClient = blobServiceClient.GetBlobContainerClient(path);

           BlobItem blobItem= containerClient.GetBlobs().LastOrDefault();

           var pathPic= Path.Combine(containerClient.Uri.ToString(), blobItem.Name);

            return pathPic;
        }

        public async Task AddImage(string file)
        {
            fileName = Path.GetFileName(file);
            blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(file, true);
        }


        public  async Task<string> UploadFile(IFormFile file)
        {
            //blobServiceClient = new BlobServiceClient(connection);

            //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(path);

           blobClient = containerClient.GetBlobClient(file.FileName);

            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            string linkName = containerClient.GetBlobs().Where(o => o.Name.Equals(file.FileName)).FirstOrDefault().Name;
            string link = Path.Combine(containerClient.Uri + "/", linkName);

            return link;
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
