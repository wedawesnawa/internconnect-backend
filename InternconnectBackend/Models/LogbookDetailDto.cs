using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models
{
    public class LogbookDetailDto
    {
        public DateTime Date { get; set; }
        public string Deskripsi { get; set; }
        public string? Kendala { get; set; }
        public string StatusAttend { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string? Status { get; set; }
    }
}
