using Microsoft.AspNetCore.Mvc;
using mp4viewernew.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Encodings.Web;

namespace mp4viewernew.Controllers
{
    public class Catalogue : Controller
    {
        private IWebHostEnvironment _env;
        private IConfiguration _config;
        private List<FileViewModel> files;

        public Catalogue(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
        )
        {
            _env = webHostEnvironment;
            _config = configuration;
            files = new List<FileViewModel>();
        }
        public IActionResult Index()
        {
            var filePaths = Directory.GetFiles(@$"{_env.WebRootPath}\{_config["ServerFolder:FolderName"]}");
            
            foreach (var file in filePaths)
            {
                files.Add(new FileViewModel
                {
                    FileName = Path.GetFileName(file)
                });
            };
            TempData["hidden"] = true;
            return View(files);
        }

        public IActionResult ViewFile(string fileName)
        {
            TempData["fileToPlay"] = $"~/{_config["ServerFolder:FolderName"]}/{fileName}";
            TempData["hidden"] = false;
            return RedirectToAction("Index", "Catalogue");
        }
    }
}
