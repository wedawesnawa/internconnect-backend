using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Models.Domain
{
    public class Logbook
    {
        [Key]
        public Guid KodeLogbook { get; set; }

        [Required]
        public required string Content { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        [Required]
        public required string Status { get; set; }

        [Required]
        public required string Deskripsi { get; set; }

        public string? ImageUrl { get; set; }

        public required string Username { get; set; }

        [ForeignKey("Username")]
        public User? User { get; set; }
        public int TotalDateRange { get; set; }
        public int TotalLogbookDetails { get; set; }
    }
}
