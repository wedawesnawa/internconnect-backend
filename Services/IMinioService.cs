using Microsoft.AspNetCore.Http;

namespace InternconnectBackend.Services
{
    public interface IMinioService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "uploads");
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<string> GetFileUrlAsync(string fileName, int expiryMinutes = 60);
        Task<byte[]> DownloadFileAsync(string fileName);
    }
}