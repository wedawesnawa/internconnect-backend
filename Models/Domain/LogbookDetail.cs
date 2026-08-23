using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models.Domain
{
    public class LogbookDetail
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public required string Deskripsi { get; set; }
        public string? Kendala { get; set; }
        public required string StatusAttend { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string? Status { get; set; }

        public Guid KodeLogbook { get; set; }

    }
}
