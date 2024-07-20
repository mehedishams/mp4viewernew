using Microsoft.AspNetCore.Mvc;
using mp4viewernew.Models;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace mp4viewernew.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(IFormFile postedFile)
    {
        if (postedFile == null || postedFile.Length == 0)
            return BadRequest("No file selected for upload...");

        string fileName = Path.GetFileName(postedFile.FileName);
        string contentType = postedFile.ContentType;

        return View();
    }

    //public Task<IActionResult> Post(List<IFormFile> battlePlans)
    //{
    //    return "";
    //}

    public IActionResult Upload()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
