using Minio;
using Minio.DataModel.Args;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InternconnectBackend.Services
{
    public class MinioService : IMinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;
        private readonly string _publicUrl;
        private readonly ILogger<MinioService> _logger;

        public MinioService(IConfiguration configuration, ILogger<MinioService> logger)
        {
            var minioConfig = configuration.GetSection("Minio");
            _bucketName = minioConfig["BucketName"];
            _publicUrl = minioConfig["PublicUrl"];
            _logger = logger;

            _minioClient = new MinioClient()
                .WithEndpoint(minioConfig["Endpoint"])
                .WithCredentials(minioConfig["AccessKey"], minioConfig["SecretKey"])
                .WithSSL(bool.Parse(minioConfig["UseSSL"]))
                .Build();

            EnsureBucketExists().GetAwaiter().GetResult();
        }

        private async Task EnsureBucketExists()
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);
                bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);

                if (!found)
                {
                    var makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
                    await _minioClient.MakeBucketAsync(makeBucketArgs);
                    _logger.LogInformation($"Bucket '{_bucketName}' created successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ensuring bucket exists: {ex.Message}");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "uploads")
        {
            try
            {
                // Validasi file
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty or null");

                // Generate unique filename dengan folder
                var extension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = $"{folder}/{uniqueFileName}".Replace("\\", "/");

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fullPath)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(file.ContentType);

                await _minioClient.PutObjectAsync(putObjectArgs);

                _logger.LogInformation($"File uploaded: {fullPath}");

                // Return path yang akan disimpan di database
                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                    return true;

                // Extract path dari URL atau path
                string objectName = fileUrl;
                if (fileUrl.Contains(_publicUrl))
                {
                    var uri = new Uri(fileUrl);
                    objectName = uri.AbsolutePath.TrimStart('/');
                }

                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName);

                await _minioClient.RemoveObjectAsync(removeObjectArgs);
                _logger.LogInformation($"File deleted: {objectName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetFileUrlAsync(string fileName, int expiryMinutes = 60)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return null;

                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName)
                    .WithExpiry(expiryMinutes * 60);

                string url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating URL: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> DownloadFileAsync(string fileName)
        {
            try
            {
                using var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await _minioClient.GetObjectAsync(getObjectArgs);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading file: {ex.Message}");
                return null;
            }
        }
    }
}