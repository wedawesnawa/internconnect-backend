using InternconnectBackend.Data;
using InternconnectBackend.Models;
using InternconnectBackend.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace InternconnectBackend.Services
{
    public class MonevService
    {
        private readonly InternconnectDbContext _context;
        private readonly WherebyService _wherebyService;
        private readonly ILogger<MonevService> _logger;

        public MonevService(InternconnectDbContext context, WherebyService wherebyService, ILogger<MonevService> logger)
        {
            _context = context;
            _wherebyService = wherebyService;
            _logger = logger;
        }

        public async Task<bool> AjukanAtauUpdateMonevAsync(Guid kodeLogbook, DateTime date, TimeSpan timeStart, TimeSpan timeEnd)
        {
            var logbookShareds = await _context.LogbookShareds
               .Where(ls => ls.KodeLogbook == kodeLogbook)
               .ToListAsync();

            if (!logbookShareds.Any())
            {
                _logger.LogWarning("Logbook dengan kode {KodeLogbook} tidak ditemukan atau tidak dibagikan ke siapa pun.", kodeLogbook);
                return false;
            }

            try
            {
                string? meetingUrl = await _wherebyService.CreateMeetingAsync(date, timeStart, timeEnd);
                if (string.IsNullOrEmpty(meetingUrl))
                {
                    _logger.LogError("Gagal membuat meeting room dari Whereby untuk logbook {KodeLogbook}.", kodeLogbook);
                    return false;
                }

                foreach (var logbookShared in logbookShareds)
                {
                    var existingMonev = await _context.Monevs.FirstOrDefaultAsync(m =>
                        m.KodeLogbook == kodeLogbook && m.IdShared == logbookShared.IdShared);

                    if (existingMonev == null)
                    {
                        var newMonev = new Monev
                        {
                            Date = date,
                            TimeStart = timeStart,
                            TimeEnd = timeEnd,
                            RoomUrl = meetingUrl,
                            KodeLogbook = kodeLogbook,
                            IdShared = logbookShared.IdShared
                        };

                        _context.Monevs.Add(newMonev);
                    }
                    else
                    {
                        existingMonev.Date = date;
                        existingMonev.TimeStart = timeStart;
                        existingMonev.TimeEnd = timeEnd;
                        existingMonev.RoomUrl = meetingUrl;

                        _context.Monevs.Update(existingMonev);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Terjadi kesalahan saat menyimpan Monev untuk logbook {KodeLogbook}: {Message}", kodeLogbook, ex.Message);
                return false;
            }
        }

        public async Task<List<Monev?>> GetMonevByKodeLogbookAsync(Guid kodeLogbook)
        {
            return await _context.Monevs.Where(m => m.KodeLogbook == kodeLogbook).Include(m => m.LogbookShared).ToListAsync() ?? new List<Monev>(); ;
        }
        public async Task<List<Logbook>> GetLogbooksWithMonevAsync(string username)
        {
            return await _context.Monevs
                .Include(m => m.Logbook)
                .ThenInclude(l => l.User)
                .Where(m => m.Logbook != null &&
                            (m.Logbook.Username == username ||
                             _context.LogbookShareds.Any(s => s.KodeLogbook == m.KodeLogbook && s.SharedWith == username)))
                .Select(m => m.Logbook!)
                .Distinct()
                .ToListAsync();
        }

    }
}