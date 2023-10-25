namespace Week7Sample.Service
{
    public interface IUploadService
    {
        Task<Dictionary<string, string>> UploadFileAsync(IFormFile file);
    }
}
