using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Week7Sample.Service
{
    public class UploadService : IUploadService
    {
        private readonly IConfiguration _config;
        public UploadService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<Dictionary<string, string>> UploadFileAsync(IFormFile file)
        {
            var account = new Account
            {
                ApiKey = _config.GetSection("CloudinarySettings:ApiKey").Value,
                ApiSecret = _config.GetSection("CloudinarySettings:ApiSecret").Value,
                Cloud = _config.GetSection("CloudinarySettings:CloudName").Value
            };


            var cloudinary = new Cloudinary(account);

            if(file.Length > 0 && file.Length <=(1024 * 1024 * 2))
            {
                if(file.ContentType.Equals("image/png") || file.ContentType.Equals("image/jpg") || file.ContentType.Equals("image/jpeg"))
                {
                    UploadResult uploadResult = new ImageUploadResult();
                    using(var stream = file.OpenReadStream())
                    {
                        var uploadParameters = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, stream),
                            Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                        };

                        uploadResult = await cloudinary.UploadAsync(uploadParameters);
                    }

                    var result = new Dictionary<string, string>();
                    result.Add("PublicId", uploadResult.PublicId);
                    result.Add("Url", uploadResult.Url.ToString());
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
