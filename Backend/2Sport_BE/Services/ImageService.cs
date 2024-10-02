using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;

namespace _2Sport_BE.Services
{
    public interface IImageService
    {
        Task<ImageUploadResult> UploadImageToCloudinaryAsync(IFormFile file);
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
    }
}
