using InternconnectBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InternconnectBackend.Controllers
{
    //[Authorize(Roles = "Dosen")]
    [Route("api/[controller]")]
    [ApiController]
    public class DosenController : ControllerBase
    {

        private readonly InternconnectDbContext _context;

        public DosenController(InternconnectDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have accessed the Dosen controller.");
        }

        [HttpGet("relation-user")]
        public async Task<IActionResult> GetSharedLogbooks()
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUsername))
            {
                return Unauthorized(new { message = "Token tidak valid atau tidak memiliki username." });
            }

            // Query untuk mendapatkan logbook yang dibagikan kepada pengguna tertentu
            var sharedLogbooks = await _context.LogbookShareds
                .Where(ls => ls.SharedWith == currentUsername)
                .Join(
                    _context.Logbooks,
                    ls => ls.KodeLogbook,
                    l => l.KodeLogbook,
                    (ls, l) => new
                    {
                        l.KodeLogbook,
                        l.Content,
                        l.DateStart,
                        l.DateEnd,
                        l.Status,
                        l.Deskripsi,
                        l.ImageUrl,
                        l.TotalDateRange,
                        l.TotalLogbookDetails,
                        OwnerUsername = l.Username, // Pemilik logbook
                        ls.SharedAt,
                        ls.Permission // Hak akses (read-only/edit)
                    })
                .ToListAsync();

            if (sharedLogbooks.Count == 0)
            {
                return NotFound(new { message = "Tidak ada logbook yang dibagikan kepada Anda." });
            }

            return Ok(sharedLogbooks);
        }

        [HttpGet("relation")]
        public async Task<IActionResult> GetRelation()
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUsername))
            {
                return Unauthorized(new { message = "Token tidak valid atau tidak memiliki username." });
            }

            // 1️⃣ Logbook yang DIBAGIKAN KE user saat ini (Siapa yang membagikan ke user ini)
            var receivedLogbooks = await _context.LogbookShareds
                .Where(ls => ls.SharedWith == currentUsername)
                .Include(ls => ls.Logbook)
                .Select(ls => new
                {
                    ls.Logbook.KodeLogbook,
                    ls.Logbook.Content,
                    ls.Logbook.DateStart,
                    ls.Logbook.DateEnd,
                    ls.Logbook.Status,
                    ls.Logbook.Deskripsi,
                    ls.Logbook.ImageUrl,
                    SharedBy = ls.Logbook.Username, // Pemilik asli logbook yang membagikan
                    ls.SharedAt,
                    ls.Permission // Hak akses (read-only/edit)
                })
                .ToListAsync();

            // 2️⃣ Logbook yang DIBAGIKAN OLEH user saat ini (Siapa yang menerima dari user ini)
            var givenLogbooks = await _context.LogbookShareds
                .Where(ls => ls.Logbook.Username == currentUsername) // Logbook yang dimiliki user ini
                .Include(ls => ls.Logbook)
                .Select(ls => new
                {
                    ls.Logbook.KodeLogbook,
                    ls.Logbook.Content,
                    ls.Logbook.DateStart,
                    ls.Logbook.DateEnd,
                    ls.Logbook.Status,
                    ls.Logbook.Deskripsi,
                    ls.Logbook.ImageUrl,
                    SharedWith = ls.SharedWith, // Orang yang menerima logbook ini
                    ls.SharedAt,
                    ls.Permission
                })
                .ToListAsync();

            return Ok(new
            {
                ReceivedFromOthers = receivedLogbooks, // Logbook yang diterima oleh user saat ini
                GivenToOthers = givenLogbooks // Logbook yang dibagikan oleh user saat ini
            });
        }

    }
}
