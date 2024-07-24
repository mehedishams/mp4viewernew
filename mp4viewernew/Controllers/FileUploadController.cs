using Microsoft.AspNetCore.Mvc;
using mp4viewernew.Utilities;

namespace mp4viewernew.Controllers;

public class FileUploadController : Controller
{
    private readonly ILogger<FileUploadController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private static readonly string[] permittedExtensions = new string[] { ".mp4" };

    public FileUploadController(
        ILogger<FileUploadController> logger,
        IConfiguration configuration,
        IWebHostEnvironment env
    )
    {
        _logger = logger;
        _configuration = configuration;
        _env = env;
    }

    /// <summary>
    /// Uploads file.
    /// </summary>    
    /// <returns></returns>
    [HttpPost]
    [Route(nameof(UploadFile))]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> UploadFile(IEnumerable<IFormFile> postedFiles)
    {
        if (postedFiles.ToList().Count == 0)
        {
            return BadRequest("No files selected.");
        }

        foreach (var postedFile in postedFiles)
        {
            if (FileHelpers.FileLengthInAcceptableRange(postedFile, 200))
                return BadRequest("File size exceeded.");

            //if (!(await FileHelpers.ValidMp4File(postedFile, permittedExtensions, _logger)))
            //    return BadRequest("Not a valid mp4 file.");

            var saveFileName = Path.GetFileName(postedFile.FileName);
            var contentType = postedFile.ContentType;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await postedFile.CopyToAsync(memoryStream);
                var contentPath = @$"{_env.WebRootPath}\{_configuration["ServerFolder:FolderName"]}";

                if (!Path.Exists(contentPath))
                {
                    Directory.CreateDirectory(contentPath);
                }
                saveFileName = @$"{contentPath}\{saveFileName}";
                FileStream file = new FileStream(saveFileName, FileMode.Create, System.IO.FileAccess.Write);
                memoryStream.WriteTo(file);
            }
        }
        return Redirect("~/Catalogue/Index");
    }
}
