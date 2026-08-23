using InternconnectBackend.Models;
using InternconnectBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternconnectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonevController : ControllerBase
    {
        private readonly MonevService _monevService;
        private readonly ILogger<MonevController> _logger;

        public MonevController(MonevService monevService, ILogger<MonevController> logger)
        {
            _monevService = monevService;
            _logger = logger;
        }

        [HttpPost("ajukan-monev")]
        public async Task<IActionResult> AjukanAtauUpdateMonev([FromBody] MonevDto request)
        {
            try
            {
                var success = await _monevService.AjukanAtauUpdateMonevAsync(
                    request.KodeLogbook, request.Date, request.TimeStart, request.TimeEnd);

                if (!success)
                {
                    return BadRequest(new { message = "Gagal mengajukan Monev. Pastikan logbook tersedia atau Whereby tidak error." });
                }

                return Ok(new { message = "Monev berhasil diajukan atau diperbarui." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Terjadi error di AjukanAtauUpdateMonev: {Message}", ex.Message);
                return StatusCode(500, new { message = "Terjadi kesalahan internal." });
            }
        }

        [HttpGet("{kodeLogbook}")]
        public async Task<IActionResult> GetMonevByKodeLogbook(Guid kodeLogbook)
        {
            var monevs = await _monevService.GetMonevByKodeLogbookAsync(kodeLogbook);

            if (monevs == null || !monevs.Any())
            {
                return NotFound(new { message = "Tidak ada data Monev untuk kode logbook ini." });
            }

            var result = monevs.Select(m => new
            {
                m.Date,
                m.TimeStart,
                m.TimeEnd,
                m.RoomUrl,
                m.KodeLogbook,
                m.IdShared,
                shared = m.LogbookShared != null ? new
                {
                    IdShared = m.LogbookShared.IdShared,
                    SharedWith = m.LogbookShared.SharedWith ?? "Unknown",
                    Permission = m.LogbookShared.Permission ?? "Unknown"
                } : null
            });

            return Ok(result);
        }

        [HttpGet("logbooks-with-monev")]
        public async Task<IActionResult> GetLogbooksWithMonev()
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name); // Ambil username dari token JWT
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "User tidak ditemukan." });
                }

                var logbooks = await _monevService.GetLogbooksWithMonevAsync(username);
                if (!logbooks.Any())
                {
                    return NotFound(new { message = "Tidak ada logbook yang berhasil diajukan Monev." });
                }

                return Ok(logbooks.Select(l => new
                {
                    l.KodeLogbook,
                    l.Content,
                    l.DateStart,
                    l.DateEnd,
                    l.Deskripsi,
                    l.Username,
                    l.ImageUrl,
                    l.Status,
                    l.TotalDateRange,
                    l.TotalLogbookDetails
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Terjadi error di GetLogbooksWithMonev: {Message}", ex.Message);
                return StatusCode(500, new { message = "Terjadi kesalahan internal." });
            }
        }

    }
}
