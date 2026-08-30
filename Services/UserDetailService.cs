using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using InternconnectBackend.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InternconnectBackend.Services
{
    public class UserDetailService
    {
        private readonly InternconnectDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMinioService _minioService; // Tambahkan ini

        public UserDetailService(
            InternconnectDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMinioService minioService) // Tambahkan parameter
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _minioService = minioService; // Inject MinIO Service
        }

        private string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        public async Task<UserDetail> GetUserDetail()
        {
            var username = GetCurrentUsername();
            return await _context.UserDetails.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserDetail> AddUserDetail(UserDetailDto userDetailDto)
        {
            var username = GetCurrentUsername();
            var userDetail = new UserDetail
            {
                Username = username,
                Nama = userDetailDto.Nama,
                Telp = userDetailDto.Telp,
                Bio = userDetailDto.Bio,
                Alamat = userDetailDto.Alamat,
                Instansi = userDetailDto.Instansi,
                AlamatInstansi = userDetailDto.AlamatInstansi
            };

            _context.UserDetails.Add(userDetail);
            await _context.SaveChangesAsync();
            return userDetail;
        }

        public async Task<UserDetail> UpdateUserDetail(UserDetailDto userDetailDto)
        {
            var username = GetCurrentUsername();
            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null)
                return null;

            userDetail.Nama = userDetailDto.Nama;
            userDetail.Telp = userDetailDto.Telp;
            userDetail.Bio = userDetailDto.Bio;
            userDetail.Alamat = userDetailDto.Alamat;
            userDetail.Instansi = userDetailDto.Instansi;
            userDetail.AlamatInstansi = userDetailDto.AlamatInstansi;

            await _context.SaveChangesAsync();
            return userDetail;
        }

        // Method untuk update profile picture dengan MinIO
        public async Task<string> UpdateProfilePictureWithMinioAsync(IFormFile file)
        {
            var username = GetCurrentUsername();
            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null)
                return null;

            try
            {
                // Hapus foto lama jika ada
                if (!string.IsNullOrEmpty(userDetail.profileUrl))
                {
                    await _minioService.DeleteFileAsync(userDetail.profileUrl);
                }

                // Upload foto baru ke MinIO dengan folder "profile-pictures"
                var imagePath = await _minioService.UploadFileAsync(file, "profile-pictures");

                // Update database dengan path baru
                userDetail.profileUrl = imagePath;
                await _context.SaveChangesAsync();

                return imagePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating profile picture: {ex.Message}");
            }
        }

        // Method untuk mendapatkan URL foto profil
        public async Task<string> GetProfilePictureUrlAsync()
        {
            var username = GetCurrentUsername();
            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null || string.IsNullOrEmpty(userDetail.profileUrl))
                return null;

            // Generate presigned URL
            return await _minioService.GetFileUrlAsync(userDetail.profileUrl);
        }

        public async Task<string?> GetProfilePictureFullUrlByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null || string.IsNullOrEmpty(userDetail.profileUrl))
                return null;

            return await _minioService.GetFileUrlAsync(userDetail.profileUrl);
        }

        public async Task<UserDetail> GetUserDetailByUsername(string username)
        {
            var userDetail = await _context.UserDetails
                .Where(u => u.Username == username)
                .Select(u => new UserDetail
                {
                    Nama = u.Nama,
                    Telp = u.Telp,
                    Bio = u.Bio,
                    Alamat = u.Alamat,
                    Instansi = u.Instansi,
                    AlamatInstansi = u.AlamatInstansi,
                    profileUrl = u.profileUrl,
                    FileUrl = u.FileUrl,
                    Username = u.Username
                })
                .FirstOrDefaultAsync();

            return userDetail;
        }

        // Method lama untuk kompatibilitas (bisa dihapus jika tidak digunakan)
        public async Task<bool> UpdateProfilePicture(string imageUrl)
        {
            var username = GetCurrentUsername();
            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userDetail == null)
                return false;

            userDetail.profileUrl = imageUrl;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}