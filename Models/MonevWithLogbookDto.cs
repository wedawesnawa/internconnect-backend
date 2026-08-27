// Models/MonevWithLogbookDto.cs
namespace InternconnectBackend.Models
{
    public class MonevWithLogbookDto
    {
        // Data dari Monev
        public int IdMonev { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string? RoomUrl { get; set; }
        public Guid KodeLogbook { get; set; }
        public int IdShared { get; set; }

        // Data dari Logbook
        public string? LogbookContent { get; set; }
        public DateTime? LogbookDateStart { get; set; }
        public DateTime? LogbookDateEnd { get; set; }
        public string? LogbookDeskripsi { get; set; }
        public string? LogbookUsername { get; set; }
        public string? LogbookImageUrl { get; set; }
        public string? LogbookStatus { get; set; }
        public int? LogbookTotalDateRange { get; set; }
        public int? LogbookTotalLogbookDetails { get; set; }

        // Data dari LogbookShared
        public string? SharedWith { get; set; }
        public string? Permission { get; set; }

        // Data dari User (optional)
        public string? UserNama { get; set; }
        public string? UserEmail { get; set; }
    }
}