using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using _2Sport_BE.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageVideoController : ControllerBase
    {
        private readonly IImageVideosService _imageVideoService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public ImageVideoController(IImageVideosService imageVideoService, IMapper mapper, IWebHostEnvironment environment)
        {
            _imageVideoService = imageVideoService;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        [Route("get-all-images-in-text-editor")]
        public async Task<IActionResult> GetAllImagesInTextEditor([FromQuery] string subfolderPath = "")
        {
            var rootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
            // Combine paths: RootPath/Media/(optional subfolderPath)/fileName
            var mediaDirectory = Path.Combine(rootPath, "Media");
            var targetDirectory = string.IsNullOrEmpty(subfolderPath)
                ? mediaDirectory
                : Path.Combine(mediaDirectory, subfolderPath);

            // Check if the directory exists
            if (!Directory.Exists(targetDirectory))
            {
                return NotFound("Images folder does not exist.");
            }

            // Get all image files in the folder
            var imageFiles = Directory.GetFiles(targetDirectory);

            // Construct URLs for each image
            var imageUrls = new List<string>();
            foreach (var filePath in imageFiles)
            {
                var fileName = Path.GetFileName(filePath);
                var fileUrl = "";
                if (subfolderPath != "")
                {
                    fileUrl = $"{Request.Scheme}://{Request.Host}/api/ImageVideo/view-image?fileName={fileName}" +
                        $"&subFolderPath={subfolderPath}"; ;
                } else
                {
                    fileUrl = $"{Request.Scheme}://{Request.Host}/api/ImageVideo/view-image?fileName={fileName}";
                }
                imageUrls.Add(fileUrl);
            }

            return Ok(imageUrls);
        }

        [HttpGet("view-image")]
        public IActionResult ViewImage([FromQuery] string fileName, [FromQuery] string subfolderPath = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            // Construct the full file path
            var mediaDirectory = Path.Combine(_environment.ContentRootPath, "Media");
            var targetDirectory = string.IsNullOrEmpty(subfolderPath) ? mediaDirectory : Path.Combine(mediaDirectory, subfolderPath);
            var filePath = Path.Combine(targetDirectory, fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            try
            {
                // Determine the content type based on file extension
                var contentType = GetContentType(filePath);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType);
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"Error reading file: {ex.Message}");
            }
        }

        private string GetContentType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

        [HttpPost]
        [Route("upload-image-in-text-editor")]
        public async Task<IActionResult> UploadImageInTextEditor(IFormFile file, [FromQuery] string subfolderPath = "")
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            string filter = "*.png,*.gif,*.jpg,*.jpeg";
            if (filter.Contains(Path.GetExtension(file.FileName)))
            {
                var fileName = Path.GetFileName(file.FileName);

                // Combine paths: RootPath/Media/(optional subfolderPath)/fileName
                var rootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
                var mediaDirectory = Path.Combine(rootPath, "Media");
                var targetDirectory = string.IsNullOrEmpty(subfolderPath)
                    ? mediaDirectory
                    : Path.Combine(mediaDirectory, subfolderPath);
                var filePath = Path.Combine(targetDirectory, fileName);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = "";
                if (subfolderPath != "")
                {
                    fileUrl = $"{Request.Scheme}://{Request.Host}/api/ImageVideo/view-image?fileName={fileName}" +
                        $"&subFolderPath={subfolderPath}";
                } else
                {
                    fileUrl = $"{Request.Scheme}://{Request.Host}/api/ImageVideo/view-image?fileName={fileName}";
                }
                return Ok(new { FileUrl = fileUrl });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Route("create-folder-in-text-editor")]
        public async Task<IActionResult> CreateFolder(string folderName)
        {
            // Get the root path of the project
            var rootPath = _environment.ContentRootPath;

            // Create a path for "RootPath/Media/YourFolderName"
            var mediaSubdirectoryPath = Path.Combine(rootPath, "Media", folderName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(mediaSubdirectoryPath))
            {
                Directory.CreateDirectory(mediaSubdirectoryPath);
                return Ok("Create folder successfully!");
            }
            return Ok("Create folder failed!");
        }

        [HttpDelete("delete-image-of-text-editor")]
        public IActionResult DeleteImage([FromQuery] string fileName, [FromQuery] string subfolderPath = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name is required.");
            }

            // Combine paths: RootPath/Media/(optional subfolderPath)/fileName
            var mediaDirectory = Path.Combine(_environment.ContentRootPath, "Media");
            var targetDirectory = string.IsNullOrEmpty(subfolderPath)
                ? mediaDirectory 
                : Path.Combine(mediaDirectory, subfolderPath); 

            var filePath = Path.Combine(targetDirectory, fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            try
            {
                // Delete the file
                System.IO.File.Delete(filePath);
                return Ok("File deleted successfully.");
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("list-all-image-in-products")]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                var imageList = (await _imageVideoService.GetAllImages()).Include(_ => _.Product).ToList();
                var result = _mapper.Map<List<ImagesVideoVM>>(imageList);
                return Ok(new { total = result.Count(), data = result });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("delete-image-video/{productId}")]
        public async Task<IActionResult> DeleteImageVideo(int productId)
        {
            try
            {
                var deletedImageVideos = (await _imageVideoService.GetImageVideosByProductId(productId)).ToList();
                if (deletedImageVideos.Count <= 0)
                {
                    return BadRequest($"Cannot find image video with product id: {productId}");
                }
                await _imageVideoService.DeleteImagesVideos(deletedImageVideos);
                return Ok("Delete image video successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
    }
}
