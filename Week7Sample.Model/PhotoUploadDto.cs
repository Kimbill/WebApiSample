using Microsoft.AspNetCore.Http;

namespace Week7Sample.Model
{
    public class PhotoUploadDto
    {
        public IFormFile File { get; set; }
    }
}
