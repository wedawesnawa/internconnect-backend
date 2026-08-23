using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InternconnectBackend.Controllers
{

    //[Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class DetailLogbookController : ControllerBase
    {
        private readonly InternconnectDbContext _context;

        public DetailLogbookController(InternconnectDbContext context)
        {
            _context = context;
        }

        [HttpPost("{kodeLogbook}/create")]
        public async Task<IActionResult> CreateDetailLogbook(Guid kodeLogbook, [FromBody] LogbookDetailDto dto)
        {
            // Cek apakah logbook dengan kode tersebut ada
            var logbook = await _context.Logbooks.FindAsync(kodeLogbook);
            if (logbook == null) return NotFound(new { message = "Logbook not found" });

            // Cek apakah ada entri dengan tanggal yang sama di logbook ini
            var existingDetail = await _context.LogbookDetails
                .FirstOrDefaultAsync(d => d.KodeLogbook == kodeLogbook && d.Date == dto.Date);

            if (existingDetail != null)
            {
                // Update data yang sudah ada
                existingDetail.Deskripsi = dto.Deskripsi;
                existingDetail.Kendala = dto.Kendala;
                existingDetail.StatusAttend = dto.StatusAttend;
                existingDetail.TimeStart = dto.TimeStart;
                existingDetail.TimeEnd = dto.TimeEnd;
                existingDetail.Status = dto.Status;

                _context.LogbookDetails.Update(existingDetail);
            }
            else
            {

                var detailLogbook = new LogbookDetail
                {
                    Date = dto.Date,
                    Deskripsi = dto.Deskripsi,
                    Kendala = dto.Kendala,
                    StatusAttend = dto.StatusAttend,
                    TimeStart = dto.TimeStart,
                    TimeEnd = dto.TimeEnd,
                    Status = dto.Status,
                    KodeLogbook = kodeLogbook
                };

                _context.LogbookDetails.Add(detailLogbook);
            }
            await _context.SaveChangesAsync();

            _context.UpdateLogbookProgress(kodeLogbook);

            return Ok(new { message = "Detail logbook berhasil diperbarui atau ditambahkan." });
        }
        // 2. Get Detail Logbook by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailLogbook(int id)
        {
            var detailLogbook = await _context.LogbookDetails.FindAsync(id);
            if (detailLogbook == null) return NotFound(new { message = "Detail logbook not found" });

            return Ok(detailLogbook);
        }

        [HttpGet("{kodeLogbook}/all")]
        public async Task<IActionResult> GetAllDetailLogbooks(Guid kodeLogbook)
        {
            var detailLogbooks = await _context.LogbookDetails
                .Where(d => d.KodeLogbook == kodeLogbook)
                .OrderBy(d => d.Date) // Urutkan berdasarkan tanggal
                .ToListAsync();

            if (!detailLogbooks.Any())
            {
                return NotFound(new { message = "Tidak ada detail logbook untuk kode ini." });
            }

            return Ok(detailLogbooks);
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateDetailLogbook(int id, [FromBody] LogbookDetailDto dto)
        {
            var detailLogbook = await _context.LogbookDetails.FindAsync(id);
            if (detailLogbook == null) return NotFound(new { message = "Detail logbook not found" });

            // Cek apakah ada entri dengan tanggal yang sama di logbook ini (kecuali dirinya sendiri)
            var existingDetail = await _context.LogbookDetails
                .FirstOrDefaultAsync(d => d.KodeLogbook == detailLogbook.KodeLogbook && d.Date == dto.Date && d.Id != id);

            if (existingDetail != null)
            {
                return BadRequest(new { message = "Detail logbook pada tanggal ini sudah ada!" });
            }

            detailLogbook.Date = dto.Date;
            detailLogbook.Deskripsi = dto.Deskripsi;
            detailLogbook.Kendala = dto.Kendala;
            detailLogbook.StatusAttend = dto.StatusAttend;
            detailLogbook.TimeStart = dto.TimeStart;
            detailLogbook.TimeEnd = dto.TimeEnd;
            detailLogbook.Status = dto.Status;

            _context.LogbookDetails.Update(detailLogbook);
            await _context.SaveChangesAsync();

            return Ok(detailLogbook);
        }

        // 4. Delete Detail Logbook
        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteDetailLogbook(int id)
        {
            var detailLogbook = await _context.LogbookDetails.FindAsync(id);
            if (detailLogbook == null) return NotFound(new { message = "Detail logbook not found" });

            _context.LogbookDetails.Remove(detailLogbook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Pembimbing")]
        [HttpPut("{id}/verif")]
        public async Task<IActionResult> VerifDetailLogbook(int id, [FromBody] VerifStatusDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new { message = "Status tidak boleh kosong" });
            }

            var detailLogbook = await _context.LogbookDetails.FindAsync(id);
            if (detailLogbook == null)
            {
                return NotFound(new { message = "Detail logbook tidak ditemukan" });
            }

            // Hanya memperbarui status
            detailLogbook.Status = dto.Status;

            _context.LogbookDetails.Update(detailLogbook);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status berhasil diperbarui", status = detailLogbook.Status });
        }

    }
}
