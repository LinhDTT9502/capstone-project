using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;

namespace _2Sport_BE.Services
{
    public interface IImageService
    {
        Task<ImageUploadResult> UploadImageToCloudinaryAsync(IFormFile file);
        Task<ImageUploadResult> UploadImageToCloudinaryAsync(IFormFile file, string folder);
        Task<List<string>> ListImagesAsync(string folderName);
        Task<bool> DeleteAnImage(string fileName);
    }
    public class ImageService : IImageService
    {
        public async Task<ImageUploadResult> UploadImageToCloudinaryAsync(IFormFile file)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        UseFilename = true,
                        UniqueFilename = false,
                        Overwrite = true
                    };
                    uploadResult = await cloudinary.UploadAsync(uploadParams);
                }
            }

            return uploadResult;
        }

        public async Task<ImageUploadResult> UploadImageToCloudinaryAsync(IFormFile file, string folder)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = folder,
                        UseFilename = true,
                        UniqueFilename = false,
                        Overwrite = true
                    };
                    uploadResult = await cloudinary.UploadAsync(uploadParams);
                }
            }

            return uploadResult;
        }

        // List All Images in a Folder
        public async Task<List<string>> ListImagesAsync(string folderName)
        {
            var imageUrls = new List<string>();
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
            var listParams = new ListResourcesByPrefixParams()
            {
                Prefix = folderName + "/",  // Cloudinary uses a "folder_name/" prefix to identify folders
                Type = "upload",            // Specify the resource type (e.g., "upload" for images/files)
                MaxResults = 100            // Limit the number of results (default: 10, max: 500)
            };

            var resources = cloudinary.ListResources(listParams);

            if (resources != null)
            {
                foreach (var resource in resources.Resources)
                {
                    imageUrls.Add(resource.SecureUrl.ToString());
                }
            }

            return imageUrls;
        }

        public async Task<bool> DeleteAnImage(string fileName)
        {
            try
            {
                // Step 1: Set up Cloudinary account credentials
                var imageUrls = new List<string>();
                Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));

                // Step 2: Specify the PublicId of the image to delete
                string publicId = $"blog_images/{fileName}"; // Example PublicId ("folder_name/file_name")

                // Step 3: Delete the image
                var deletionParams = new DeletionParams(publicId)
                {
                    // Optional: Specify the resource type (default is "image")
                    ResourceType = ResourceType.Image
                };

                var deletionResult = cloudinary.Destroy(deletionParams);

                // Step 4: Output the result
                if (deletionResult.Result == "ok")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }


        }
    }
}
