using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InternconnectBackend.Controllers
{
    [Route("api/[controller]/{kodeLogbook}/")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly InternconnectDbContext _context;
        public SharedController(InternconnectDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShared(Guid kodeLogbook, [FromBody] SharedDto sharedDto)
        {
            var logbook = await _context.Logbooks.FindAsync(kodeLogbook);
            if (logbook == null)
            {
                return NotFound("Logbook not found.");
            }

            var shared = new LogbookShared
            {
                KodeLogbook = kodeLogbook,
                SharedWith = sharedDto.SharedWith,
                Permission = sharedDto.Permission,
                SharedAt = DateTime.UtcNow
            };

            _context.LogbookShareds.Add(shared);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllSharedByLogbook), new { kodeLogbook, id = shared.IdShared }, shared);
        }

        // UPDATE: api/Shared/{kodeLogbook}/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateShared(Guid kodeLogbook, int id, [FromBody] SharedDto sharedDto)
        {
            var shared = await _context.LogbookShareds.FirstOrDefaultAsync(s => s.IdShared == id && s.KodeLogbook == kodeLogbook);
            if (shared == null)
            {
                return NotFound("Shared entry not found.");
            }

            shared.SharedWith = sharedDto.SharedWith;
            shared.Permission = sharedDto.Permission;

            _context.LogbookShareds.Update(shared);
            await _context.SaveChangesAsync();

            return Ok(shared);
        }

        // DELETE: api/Shared/{kodeLogbook}/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteShared(Guid kodeLogbook, int id)
        {
            var shared = await _context.LogbookShareds.FirstOrDefaultAsync(s => s.IdShared == id && s.KodeLogbook == kodeLogbook);
            if (shared == null)
            {
                return NotFound("Shared entry not found.");
            }

            _context.LogbookShareds.Remove(shared);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSharedByLogbook(Guid kodeLogbook)
        {
            var sharedEntries = await _context.LogbookShareds
                .Where(s => s.KodeLogbook == kodeLogbook)
                .Select(s => new SharedDto
                {
                    IdShared = s.IdShared,
                    SharedWith = s.SharedWith,
                    Permission = s.Permission
                })
                .ToListAsync();

            if (!sharedEntries.Any())
            {
                return NotFound("No shared entries found for this Logbook.");
            }

            return Ok(sharedEntries);
        }
    }
}
