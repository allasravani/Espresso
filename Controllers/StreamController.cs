using ESPRESSO.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

[Route("api/stream")]
[ApiController]
public class StreamController : ControllerBase
{
    private readonly AppContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

    public StreamController(AppContext dbContext, IHttpContextAccessor httpContextAccessor, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _hostingEnvironment = hostingEnvironment;
    }
    [HttpPost("GenerateStream")]
    public IActionResult GenerateStream(string  streamName, string imageUrl = null,string emailId = null)
    {
        if ( string.IsNullOrEmpty(streamName))
        {
            return BadRequest("Invalid input data.");
        }

       
        // Set default image path if imageUrl is not provided
        string defaultImagePath = streamName;
        string imagePath = imageUrl ?? defaultImagePath;
        string webRootPath = _hostingEnvironment.WebRootPath;
        //string filePath1 = $"{streamName}";
        string fileLocation = webRootPath+@"\ConferenceCaptioningWeb";
        DirectoryInfo di = new DirectoryInfo(fileLocation);
        //String fileLocation1 = @"C:/Users/srava/Desktop/ConferenceCaptioningWeb/{filePath1}";
        // Create the directory only if it does not already exist.
        if (di.Exists == false)
            di.Create();
        
        if (!validateStreamNameExits(Path.Combine(di.FullName.ToString(),streamName)))
        {
            DirectoryInfo dis = di.CreateSubdirectory(streamName);
            DirectoryInfo requiredFiles = new DirectoryInfo(@".\Required Files\");
            String pathOfDirectory = createAndCopyDirectories(dis.FullName, requiredFiles);
            // Generate HTML template 
            string htmlTemplate = GenerateHtmlFileTemplate(streamName, imagePath);

            // Save the HTML content to a file 
            
            string htmlFilePath = Path.Combine(pathOfDirectory, "index.html");
           System.IO.File.WriteAllText(htmlFilePath, htmlTemplate);
            
           // string relativePath = Path.Combine(@".\ConferenceCaptioningWeb\",streamName,"index.html");
            // Add stream name and email ID to the database
            AddStreamToDatabase(streamName, emailId, htmlFilePath);
            string pathforaccess = @"ConferenceCaptioningWeb/"+ streamName+ "/index.html";
            string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            var absUrl = string.Format("{0}://{1}/{2}", scheme,
            host, pathforaccess);
            return successResponse(new { FileLocation = absUrl });
        }
        else
        {
            return errorResponse("Stream Name already exists.", 400);
        }
    }

    [HttpPost("GetAccessUrl")]
    public async Task<IActionResult> GetAccessUrlAsync([FromBody] RequestData requestData)
    {
        try
        {
            string url = requestData.url;
            string[] streamName = url.Split('/');
            //var urlDatas = await _dbContext.PageCounters.FirstOrDefaultAsync(x => x.Url == url);
            var urlDatas = await _dbContext.PageCounters.FirstOrDefaultAsync(x => x.STREAM_NAME == streamName[4]);
            if (urlDatas != null)
            {
                // URL exists, increment the count
                urlDatas.COUNT++;
                urlDatas.STREAM_NAME = streamName[4];
            }
            else
            {
                // URL does not exist, add it to the database
                var newUrlData = new UrlData { Url = url, COUNT = 1, STREAM_NAME = streamName[4] };
                _dbContext.PageCounters.Add(newUrlData);
            }

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return Ok(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    private IActionResult successResponse(object data)
    {
        return new JsonResult(new { StatusCode = 200, Data = data });
    }

    private IActionResult errorResponse(string message, int statusCode)
    {
        return new JsonResult(new { StatusCode = statusCode, Message = message });
    }

    [NonAction]
    private string createAndCopyDirectories(string di,DirectoryInfo requiredFiles)
    {
        
        // Create a subdirectory in the directory just created.
        
        DirectoryInfo[] dirs = requiredFiles.GetDirectories();
        
            //string targetFilePath = Path.Combine(dis.FullName, subDir.Name);
            foreach (FileInfo file in requiredFiles.GetFiles())
            {
                string targetFilePath = Path.Combine(di, file.Name);            
                file.CopyTo(targetFilePath);
            }
        foreach (DirectoryInfo subDir in dirs)
        {

            string newDestinationDir = Path.Combine(di, subDir.Name);
            Directory.CreateDirectory(newDestinationDir);
            createAndCopyDirectories(newDestinationDir,subDir);
        }
            return di;
    }
    [NonAction]
    private string GenerateHtmlFileTemplate(string streamName, string imagePath)
    {
        string htmlFilePath = @".\Required Files\index.html";

        // Read the HTML content from the file
        string htmlTemplate = System.IO.File.ReadAllText(htmlFilePath);
        if (imagePath == streamName)
        {
            string? imagePathName = dynamicLogoUrl(streamName);

            if (System.IO.File.Exists(imagePathName))
            {
                imagePath = $"./images/{Path.GetFileName(imagePathName)}";
                // Replace the {{logoPath}} placeholder with the actual image path
                htmlTemplate = htmlTemplate.Replace("{{logoPath}}", imagePath);
            }
            else
            {
                // Handle the case where the image doesn't exist
                // You can provide a default image path or handle it as needed
                htmlTemplate = htmlTemplate.Replace("{{logoPath}}", "./images/logo-wetech.png");
            }
        }
        else
        {
            htmlTemplate = htmlTemplate.Replace("{{logoPath}}", imagePath);
        }

        // htmlTemplate = htmlTemplate.Replace("{{logoPath}}", "./images/logo-wetech.png");
        return htmlTemplate;

    }
    [NonAction]
    private bool validateStreamNameExits(string fileLocation)
    {
        //FileInfo fInfo = new FileInfo(fileLocation); 
        if (!Directory.Exists(fileLocation))
        {
            return false;
        }
        return true;

    }
    [NonAction]
    private string? dynamicLogoUrl(string imageName)
    {
        string imageFolder = @"./Required Files/images";

        // Ensure the directory exists
        if (!Directory.Exists(imageFolder))
        {
            Directory.CreateDirectory(imageFolder);
        }

        string? matchingFile = Directory.GetFiles(imageFolder)
            .FirstOrDefault(file =>
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                return fileNameWithoutExtension != null &&
                       fileNameWithoutExtension.Equals(imageName, StringComparison.OrdinalIgnoreCase);
            });

        // Return the relative path if a matching file is found
        return matchingFile;
    }
    [NonAction]
    private void AddStreamToDatabase(string streamName, string emailId, string htmlFilePath)
    {

        var newStream = new UrlData
        {
            STREAM_NAME = streamName,
            EMAIL = emailId,
            Url = htmlFilePath
           // LAST_UPDATED_DATE_AND_TIME = DateTime.UtcNow
        };

        _dbContext.PageCounters.Add(newStream);
        _dbContext.SaveChanges();
    }
    
}
