using CheckFhoto_CognitiveService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Bot.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace CheckFhoto_CognitiveService.Controllers
{
    public class HomeController : Controller
    {
       
        public static string  endPoint = "https://myoliinykcognservice.cognitiveservices.azure.com/";

        public static string key = "9d7fca2694bb4aa1993d04f50cea6184";


        ComputerVisionClient computerVisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
        {
            Endpoint = endPoint
        };

        public AppContextt context { get; set; }
        public ComputerVisionClient ComputerVisionClient { get => computerVisionClient; set => computerVisionClient = value; }

        public HomeController(AppContextt applicationContext)
        {
            context = applicationContext;

        }

        public async Task<IActionResult> Index()
        {
            //await AzureService.TryCreateBlobContainer("home");

            var images = context.Pictures.AsNoTracking().ToList();

            ViewBag.images = images;


            return View();
        }

        public async Task<IActionResult>CheckPhoto(int id)
        {
          Picture pic=  context.Pictures.Where(p => p.Id == id).FirstOrDefault();
            
            string picPath = pic.Path;


            if (picPath != null)
            {
                List<VisualFeatureTypes?> features = Enum.GetValues(typeof(VisualFeatureTypes)).OfType<VisualFeatureTypes?>().ToList(); //колекція характеристик за якими можна аналізувати зображення

                //List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>() { VisualFeatureTypes.Tags };

                ImageAnalysis imageAnalysis = await computerVisionClient.AnalyzeImageAsync(picPath, features);

                ViewBag.IsAdultContent=imageAnalysis.Adult.IsAdultContent;
                ViewBag.IsRacyContent = imageAnalysis.Adult.IsRacyContent;
               ViewBag.RacyScore=imageAnalysis.Adult.RacyScore;
            }
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> UploadImageAsync(IFormFile? image)
        {
            string? extension = Path.GetExtension(image?.FileName);

            string[] exs = { ".jpg", ".bmp", ".jpeg", ".jfif", ".webp" };

            if (Array.IndexOf(exs, extension) == -1)
            {
                return View("Index");
            }

            var currentImage = context.Pictures.FirstOrDefault(i => i.Name.Equals(image.FileName));

            if (currentImage != null)
            {
                return RedirectToAction("Index");
            }

            await ClassService.UploadFile(image!);

            await context.AddImage(image!);

            await ClassService.DownloadFile(image!.FileName);

            return RedirectToAction("Index");
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