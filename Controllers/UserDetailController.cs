using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using InternconnectBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace InternconnectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailController : ControllerBase
    {
        private readonly UserDetailService _userDetailService;
        private readonly IMinioService _minioService;

        public UserDetailController(UserDetailService userDetailService, IMinioService minioService)
        {
            _userDetailService = userDetailService;
            _minioService = minioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetail()
        {
            var userDetail = await _userDetailService.GetUserDetail();
            if (userDetail == null)
            {
                return NotFound(new { message = "User detail tidak ditemukan" });
            }
            return Ok(userDetail);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserDetail([FromBody] UserDetailDto userDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetail = await _userDetailService.AddUserDetail(userDetailDto);
            return CreatedAtAction(nameof(GetUserDetail), new { id = userDetail.UserId }, userDetail);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserDetail([FromBody] UserDetailDto userDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUserDetail = await _userDetailService.UpdateUserDetail(userDetailDto);
            if (updatedUserDetail == null)
            {
                return NotFound(new { message = "User detail tidak ditemukan" });
            }
            return Ok(updatedUserDetail);
        }

        // PERBAIKAN: Ubah dari POST ke PUT dan gunakan MinIO
        [HttpPut("upload-profile-picture")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile file)
        {
            try
            {
                // Validasi file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "File tidak boleh kosong" });
                }

                // Validasi tipe file (hanya gambar)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = "Format file tidak didukung. Gunakan: JPG, JPEG, PNG, GIF, atau WEBP" });
                }

                // Validasi ukuran file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Ukuran file maksimal 5MB" });
                }

                // Upload ke MinIO
                var imagePath = await _userDetailService.UpdateProfilePictureWithMinioAsync(file);

                if (string.IsNullOrEmpty(imagePath))
                {
                    return NotFound(new { message = "User tidak ditemukan" });
                }

                // Generate presigned URL untuk akses gambar
                var imageUrl = await _userDetailService.GetProfilePictureUrlAsync();

                return Ok(new
                {
                    message = "Foto profil berhasil diperbarui",
                    profileUrl = imageUrl,
                    path = imagePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserDetail(string username)
        {
            var userDetail = await _userDetailService.GetUserDetailByUsername(username);
            if (userDetail == null)
            {
                return NotFound(new { message = "User detail tidak ditemukan" });
            }

            // Generate full URL dari MinIO jika ada profile picture
            if (!string.IsNullOrEmpty(userDetail.profileUrl))
            {
                var fullUrl = await _userDetailService.GetProfilePictureFullUrlByUsernameAsync(username);
                userDetail.profileUrl = fullUrl; // Update dengan full URL
            }
            return Ok(userDetail);
        }

        // Endpoint baru untuk mendapatkan URL foto profil
        [HttpGet("profile-picture-url")]
        public async Task<IActionResult> GetProfilePictureUrl()
        {
            try
            {
                var imageUrl = await _userDetailService.GetProfilePictureUrlAsync();

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return NotFound(new { message = "Foto profil tidak ditemukan" });
                }

                return Ok(new { profileUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("download-file")]
        public async Task<IActionResult> DownloadFile([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return BadRequest(new { message = "File path is required" });

                Console.WriteLine($"Downloading file with path: {filePath}");

                // Download dari MinIO menggunakan full path
                var fileBytes = await _minioService.DownloadFileAsync(filePath);

                if (fileBytes == null || fileBytes.Length == 0)
                    return NotFound(new { message = "File not found" });

                // Get filename from path
                var fileName = Path.GetFileName(filePath);

                // Get content type
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = GetContentType(extension);

                // Return file
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}