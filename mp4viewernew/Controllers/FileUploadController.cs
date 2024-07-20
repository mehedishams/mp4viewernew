using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using mp4viewernew.Utilities;
using System.IO;
using static mp4viewernew.Filters.ModelBinding;

namespace mp4viewernew.Controllers;

public class FileUploadController : Controller
{
    private readonly ILogger<FileUploadController> _logger;
    private readonly IConfiguration _configuration;

    public FileUploadController(
        ILogger<FileUploadController> logger,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Uploads file.
    /// </summary>    
    /// <returns></returns>
    [HttpPost]
    [Route(nameof(UploadFile))]
    //[DisableFormValueModelBindingAttribute]
    public async Task<IActionResult> UploadFile(IEnumerable<IFormFile> postedFiles)
    {
        //return BadRequest("No file selected.");

        foreach (var postedFile in postedFiles)
        {
            //if (FileHelpers.FileLengthInAcceptableRange(postedFile, 200))
            //    return BadRequest("No file selected.");
            var saveFileName = Path.GetFileName(postedFile.FileName);
            var contentType = postedFile.ContentType;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await postedFile.CopyToAsync(memoryStream);
                var tempPath = Path.GetTempPath() + _configuration["ServerFolder:FolderName"];
                                
                if (!Path.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath + @"\" + _configuration["ServerFolder:FolderName"]);
                }
                saveFileName = @$"{tempPath}\{saveFileName}";                
                FileStream file = new FileStream(saveFileName, FileMode.Create, System.IO.FileAccess.Write);
                memoryStream.WriteTo(file);
            }
        }
        return Ok();



        //var request = HttpContext.Request;

        //// validation of Content-Type
        //// 1. first, it must be a form-data request
        //// 2. a boundary should be found in the Content-Type
        //if (!request.HasFormContentType ||
        //    !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
        //    string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        //{
        //    return new UnsupportedMediaTypeResult();
        //}

        //var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
        //var reader = new MultipartReader(boundary, request.Body);
        //var section = await reader.ReadNextSectionAsync();

        //// This sample try to get the first file from request and save it
        //// Make changes according to your needs in actual use
        //while (section != null)
        //{
        //    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
        //        out var contentDisposition);

        //    if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
        //        !string.IsNullOrEmpty(contentDisposition.FileName.Value))
        //    {
        //        // Don't trust any file name, file extension, and file data from the request unless you trust them completely
        //        // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
        //        // In short, it is necessary to restrict and verify the upload
        //        // Here, we just use the temporary folder and a random file name

        //        // Get the temporary folder, and combine the filename with it.
        //        DirectoryInfo? dir = null;
        //        var tempPath = Path.GetTempPath();
        //        if (!Directory.Exists(tempPath + _configuration["ServerFolder:FolderName"]))
        //        {
        //            dir = Directory.CreateDirectory(tempPath + @"\" + _configuration["ServerFolder:FolderName"]);
        //        }
        //        var fileName = dir?.FullName + @"\test.mp4";
        //        var saveToPath = Path.Combine(Path.GetTempPath(), fileName);

        //        using (var targetStream = System.IO.File.Create(saveToPath))
        //        {
        //            await section.Body.CopyToAsync(targetStream);
        //        }

        //        return Ok();
        //    }

        //    section = await reader.ReadNextSectionAsync();
        //}

        // If the code runs to this location, it means that no files have been saved
        return BadRequest("No files data in the request.");
    }
}
