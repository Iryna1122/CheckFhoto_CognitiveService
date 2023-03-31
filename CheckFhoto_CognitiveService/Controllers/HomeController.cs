using CheckFhoto_CognitiveService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Bot.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Drawing.Imaging;

namespace CheckFhoto_CognitiveService.Controllers
{
    public class HomeController : Controller
    {
       
        public static string  endPoint = "https://myoliinykcognservice.cognitiveservices.azure.com/";
        public static string key = "9d7fca2694bb4aa1993d04f50cea6184";
        private static string path = "home";
       
        ClassService classService=new ClassService();

        public AppContextt context;


        ComputerVisionClient computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endPoint
        };


        public ComputerVisionClient ComputerVisionClient { get => computerVisionClient; set => computerVisionClient = value; }

        public HomeController(AppContextt applicationContext)
        {
            context = applicationContext;

        }

        public async Task<IActionResult> Index()
        {
           // await ClassService.TryCreateBlobContainer(path);
           // ViewBag.images= await ClassService.GetFile();


            var images = context.Pictures.ToList();

            ViewBag.images = images;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchImage(string searchString)
        {
            List<Picture> images = new List<Picture>();
            List<Picture> dbImages = context.Pictures.ToList();
            foreach (Picture item in dbImages)
            {
                if (item.Description!.ToLower().Contains(searchString))
                {
                    images.Add(item);
                }
            }
            if (images.Count > 0)
            {
                ViewBag.Imgs = images.ToList();
            }
            return View("Myphotos", images);
        }


        [HttpPost]
        public async Task<IActionResult> UploadImageAsync(IFormFile? image)
        {
            
                string folderPath = $@"{Directory.GetCurrentDirectory()}\wwwroot\img";
                Directory.CreateDirectory(folderPath);
                string filePath = Path.Combine(folderPath, image.FileName);

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    image.CopyTo(fs);
                }

                await classService.AddImage(filePath);
                string imageLink = await classService.UploadFile(image);
                ViewBag.Link = imageLink;

               
            //Check Photo On Vision
                List<VisualFeatureTypes?> featureTypes = Enum.GetValues(typeof(VisualFeatureTypes)).OfType<VisualFeatureTypes?>().ToList();

                ImageAnalysis analysis = await computerVisionClient.AnalyzeImageAsync(imageLink, featureTypes);

                if (analysis.Adult.IsAdultContent)
                {
                    ViewBag.Link = "./img/prohibited.jpg";

                }
                string imgTitle = "";
                foreach (var item in analysis.Categories)
                {
                    imgTitle += item.Name;
                    foreach (var subitem in analysis.Brands)
                    {
                        imgTitle += subitem.Name;
                    }
                }
                //count++;
                Picture temp = new Picture();

                temp.Id = Guid.NewGuid().ToString();
                temp.Name = image.FileName;
                temp.Description = imgTitle;
                temp.Path = imageLink;
                context.Add(temp);
                context.SaveChanges();

                return View("Index");
            
        }



        public IActionResult Privacy()
        {
            return View();
        }









        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}