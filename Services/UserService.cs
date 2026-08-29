using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Services; // Tambahkan ini
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InternconnectBackend.Services
{
    public class UserService
    {
        private readonly InternconnectDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMinioService _minioService; // Tambahkan ini

        // Update constructor
        public UserService(
            InternconnectDbContext context,
            IWebHostEnvironment env,
            IMinioService minioService) // Tambahkan parameter
        {
            _context = context;
            _env = env;
            _minioService = minioService; // Inject MinIO Service
        }

        public async Task<bool> UpdateUserRoleAsync(ClaimsPrincipal userClaims, UpdateUserRoleDto dto)
        {
            string? username = userClaims.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                throw new Exception("User not authenticated");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new Exception("User not found");

            user.Role = dto.NewRole;
            await _context.SaveChangesAsync();

            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.Username == username);
            if (userDetail == null)
                throw new Exception("User detail not found");

            userDetail.Instansi = dto.Instansi;

            // Upload file ke MinIO
            if (dto.File != null)
            {
                try
                {
                    // Validasi file
                    if (dto.File.Length > 10 * 1024 * 1024) // 10MB
                        throw new Exception("File size exceeds 10MB");

                    if (!dto.File.ContentType.Equals("application/pdf"))
                        throw new Exception("Invalid file format. Only PDF allowed");

                    // Hapus file lama jika ada
                    if (!string.IsNullOrEmpty(userDetail.FileUrl))
                    {
                        await _minioService.DeleteFileAsync(userDetail.FileUrl);
                    }

                    // Upload file baru ke MinIO
                    string filePath = await _minioService.UploadFileAsync(dto.File, "user-documents");
                    userDetail.FileUrl = filePath; // Simpan path di database
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error uploading file: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Method baru untuk mendapatkan file URL
        public async Task<string> GetUserFileUrlAsync(string username)
        {
            var userDetail = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.Username == username);
            if (userDetail == null || string.IsNullOrEmpty(userDetail.FileUrl))
                return null;

            return await _minioService.GetFileUrlAsync(userDetail.FileUrl);
        }

        public async Task<List<UserRole>> GetUsersByRoleAsync(string role)
        {
            var users = await _context.Users
                .Where(u => u.Role == role)
                .Select(u => new UserRole
                {
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();

            return users;
        }
    }
}