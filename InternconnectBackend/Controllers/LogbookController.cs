using InternconnectBackend.Data;
using InternconnectBackend.Models.Domain;
using InternconnectBackend.Models;
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
        public LogbookController(InternconnectDbContext dbContext)
        {
            _context = dbContext;
        }

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateLogbook([FromBody] LogbookDto model)
        //{
        //    var username = User.FindFirstValue(ClaimTypes.Name); // Mendapatkan username dari token
        //    if (username == null) return Unauthorized(new { message = "User not authenticated" });

        //    var logbook = new Logbook
        //    {
        //        Content = model.Content,
        //        DateStart = model.DateStart,
        //        DateEnd = model.DateEnd,
        //        Status = model.Status,
        //        Deskripsi = model.Deskripsi,
        //        Username = username
        //    };

        //    _context.Logbooks.Add(logbook);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Logbook created successfully", data = logbook });
        //}
        [HttpPost("create")]
        public async Task<IActionResult> CreateLogbook([FromForm] LogbookDto model)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null) return Unauthorized(new { message = "User not authenticated" });

            string? filePath = null;

            // Simpan gambar jika ada
            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                filePath = Path.Combine("uploads", uniqueFileName);

                var fullFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }

            var logbook = new Logbook
            {
                Content = model.Content,
                DateStart = model.DateStart,
                DateEnd = model.DateEnd,
                Status = model.Status,
                Deskripsi = model.Deskripsi,
                ImageUrl = filePath, // Simpan path gambar ke database
                Username = username
            };

            _context.Logbooks.Add(logbook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logbook created successfully", data = logbook });
        }


        // Mengupdate Logbook
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

            string? filePath = logbook.ImageUrl; // Gunakan gambar lama jika tidak diubah

            // Cek jika ada file gambar yang diupload
            if (model.Image != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { message = "Invalid file type. Only JPG and PNG are allowed." });

                if (model.Image.Length > 10 * 1024 * 1024) // 10MB
                    return BadRequest(new { message = "File size must be less than 10MB." });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Hapus file lama jika ada
                if (!string.IsNullOrEmpty(logbook.ImageUrl))
                {
                    var oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", logbook.ImageUrl);
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                // Simpan file baru
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                filePath = Path.Combine("uploads", uniqueFileName);
                var fullFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }

            // Update data logbook
            logbook.Content = model.Content;
            logbook.DateStart = model.DateStart;
            logbook.DateEnd = model.DateEnd;
            logbook.Status = model.Status;
            logbook.Deskripsi = model.Deskripsi;
            logbook.ImageUrl = filePath; // Update gambar jika ada perubahan

            _context.Logbooks.Update(logbook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logbook updated successfully", data = logbook });
        }


        // Menghapus Logbook
        [HttpDelete("delete/{kodeLogbook}")]
        public async Task<IActionResult> DeleteLogbook(Guid kodeLogbook)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null) return Unauthorized(new { message = "User not authenticated" });

            var logbook = await _context.Logbooks.FirstOrDefaultAsync(l => l.KodeLogbook == kodeLogbook && l.Username == username);
            if (logbook == null) return NotFound(new { message = "Logbook not found or unauthorized" });

            _context.Logbooks.Remove(logbook);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Logbook deleted successfully" });
        }



        [HttpGet("all")]
        public async Task<IActionResult> GetAllLogbooks()
        {
            var logbooks = await _context.Logbooks.ToListAsync();
            return Ok(logbooks);
        }


        [HttpGet("my-logbooks")]
        public async Task<IActionResult> GetUserLogbooks()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null) return Unauthorized(new { message = "User not authenticated" });

            var logbooks = await _context.Logbooks.Where(l => l.Username == username).ToListAsync();
            return Ok(logbooks);
        }


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

            return Ok(new { message = "Logbook retrieved successfully", data = logbook });
        }


        // Mendapatkan semua logbook berdasarkan username tertentu (Admin Only)
        [HttpGet("by-user/{username}")]
        [Authorize(Roles = "Admin")] // Hanya admin yang bisa akses
        public async Task<IActionResult> GetLogbooksByUsername(string username)
        {
            var logbooks = await _context.Logbooks.Where(l => l.Username == username).ToListAsync();
            return Ok(logbooks);
        }

    }
}
