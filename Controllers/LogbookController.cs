using InternconnectBackend.Data;
using InternconnectBackend.Models.Domain;
using InternconnectBackend.Models;
using InternconnectBackend.Services; // Tambahkan ini
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;

namespace InternconnectBackend.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [EnableCors("AllowAngular")]
    [ApiController]
    public class LogbookController : ControllerBase
    {
        private readonly InternconnectDbContext _context;
        private readonly IMinioService _minioService; // Tambahkan ini

        public LogbookController(InternconnectDbContext dbContext, IMinioService minioService)
        {
            _context = dbContext;
            _minioService = minioService; // Inject MinioService
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLogbook([FromForm] LogbookDto model)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
                return Unauthorized(new { message = "User not authenticated" });

            string? imagePath = null;

            // Upload gambar ke MinIO
            if (model.Image != null)
            {
                try
                {
                    imagePath = await _minioService.UploadFileAsync(model.Image, "logbook-images");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error uploading image: {ex.Message}" });
                }
            }

            var logbook = new Logbook
            {
                Content = model.Content,
                DateStart = model.DateStart,
                DateEnd = model.DateEnd,
                Status = model.Status,
                Deskripsi = model.Deskripsi,
                ImageUrl = imagePath, // Simpan path gambar ke database
                Username = username
            };

            _context.Logbooks.Add(logbook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logbook created successfully", data = logbook });
        }

        [HttpPut("update/{kodeLogbook}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateLogbook(Guid kodeLogbook, [FromForm] LogbookDto model)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
                return Unauthorized(new { message = "User not authenticated" });

            var logbook = await _context.Logbooks
                .FirstOrDefaultAsync(l => l.KodeLogbook == kodeLogbook && l.Username == username);

            if (logbook == null)
                return NotFound(new { message = "Logbook not found or unauthorized" });

            string? imagePath = logbook.ImageUrl; // Gunakan gambar lama

            // Cek jika ada file gambar yang diupload
            if (model.Image != null)
            {
                try
                {
                    // Hapus file lama dari MinIO
                    if (!string.IsNullOrEmpty(logbook.ImageUrl))
                    {
                        await _minioService.DeleteFileAsync(logbook.ImageUrl);
                    }

                    // Upload file baru ke MinIO
                    imagePath = await _minioService.UploadFileAsync(model.Image, "logbook-images");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error updating image: {ex.Message}" });
                }
            }

            // Update data logbook
            logbook.Content = model.Content;
            logbook.DateStart = model.DateStart;
            logbook.DateEnd = model.DateEnd;
            logbook.Status = model.Status;
            logbook.Deskripsi = model.Deskripsi;
            logbook.ImageUrl = imagePath;

            _context.Logbooks.Update(logbook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logbook updated successfully", data = logbook });
        }

        [HttpDelete("delete/{kodeLogbook}")]
        public async Task<IActionResult> DeleteLogbook(Guid kodeLogbook)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
                return Unauthorized(new { message = "User not authenticated" });

            var logbook = await _context.Logbooks.FirstOrDefaultAsync(l => l.KodeLogbook == kodeLogbook && l.Username == username);
            if (logbook == null)
                return NotFound(new { message = "Logbook not found or unauthorized" });

            // Hapus gambar dari MinIO
            if (!string.IsNullOrEmpty(logbook.ImageUrl))
            {
                await _minioService.DeleteFileAsync(logbook.ImageUrl);
            }

            _context.Logbooks.Remove(logbook);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Logbook deleted successfully" });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllLogbooks()
        {
            var logbooks = await _context.Logbooks.ToListAsync();

            // Generate presigned URLs untuk setiap gambar
            foreach (var logbook in logbooks)
            {
                if (!string.IsNullOrEmpty(logbook.ImageUrl))
                {
                    logbook.ImageUrl = await _minioService.GetFileUrlAsync(logbook.ImageUrl);
                }
            }

            return Ok(logbooks);
        }

        [HttpGet("my-logbooks")]
        public async Task<IActionResult> GetUserLogbooks()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
                return Unauthorized(new { message = "User not authenticated" });

            var logbooks = await _context.Logbooks.Where(l => l.Username == username).ToListAsync();

            // Generate presigned URLs untuk setiap gambar
            foreach (var logbook in logbooks)
            {
                if (!string.IsNullOrEmpty(logbook.ImageUrl))
                {
                    logbook.ImageUrl = await _minioService.GetFileUrlAsync(logbook.ImageUrl);
                }
            }

            return Ok(logbooks);
        }

        [Authorize(Policy = "UserOrMentorOrSupervisor")]
        [HttpGet("{kodeLogbook}")]
        public async Task<IActionResult> GetLogbookByKode(Guid kodeLogbook)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
                return Unauthorized(new { message = "User not authenticated" });

            var logbook = await _context.Logbooks
                .FirstOrDefaultAsync(l => l.KodeLogbook == kodeLogbook && l.Username == username);

            if (logbook == null)
                return NotFound(new { message = "Logbook not found or unauthorized" });

            // Generate presigned URL untuk gambar
            if (!string.IsNullOrEmpty(logbook.ImageUrl))
            {
                logbook.ImageUrl = await _minioService.GetFileUrlAsync(logbook.ImageUrl);
            }

            return Ok(new { message = "Logbook retrieved successfully", data = logbook });
        }

        [HttpGet("by-user/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLogbooksByUsername(string username)
        {
            var logbooks = await _context.Logbooks.Where(l => l.Username == username).ToListAsync();

            // Generate presigned URLs untuk setiap gambar
            foreach (var logbook in logbooks)
            {
                if (!string.IsNullOrEmpty(logbook.ImageUrl))
                {
                    logbook.ImageUrl = await _minioService.GetFileUrlAsync(logbook.ImageUrl);
                }
            }

            return Ok(logbooks);
        }

        // Endpoint tambahan: Get image URL saja
        [HttpGet("image-url/{kodeLogbook}")]
        public async Task<IActionResult> GetImageUrl(Guid kodeLogbook)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
                return Unauthorized(new { message = "User not authenticated" });

            var logbook = await _context.Logbooks
                .FirstOrDefaultAsync(l => l.KodeLogbook == kodeLogbook && l.Username == username);

            if (logbook == null || string.IsNullOrEmpty(logbook.ImageUrl))
                return NotFound(new { message = "Image not found" });

            var imageUrl = await _minioService.GetFileUrlAsync(logbook.ImageUrl);
            return Ok(new { imageUrl });
        }
    }
}