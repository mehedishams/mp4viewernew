using Microsoft.AspNetCore.Mvc;
using mp4viewernew.Models;

namespace mp4viewernew.Controllers;

public class Catalogue : Controller
{
    private IWebHostEnvironment _env;
    private IConfiguration _config;        

    public Catalogue(
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration
    )
    {
        _env = webHostEnvironment;
        _config = configuration;            
    }

    public IActionResult Index()
    {
        var filePaths = Directory.GetFiles(@$"{_env.WebRootPath}\{_config["ServerFolder:FolderName"]}");
        List<FileViewModel> files = new();
        
        foreach (var file in filePaths)
        {
            files.Add(new FileViewModel
            {
                FileName = Path.GetFileName(file)
            });
        };
        TempData["emptyCatalogue"] = files.Count == 0;
        return View(files);
    }

    public IActionResult ViewFile(string fileName)
    {
        TempData["fileToPlay"] = $"~/{_config["ServerFolder:FolderName"]}/{fileName}";
        TempData["emptyCatalogue"] = false;
        return RedirectToAction("Index", "Catalogue");
    }
}
